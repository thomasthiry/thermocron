using System.Net.Http.Headers;
using System.Text.Json;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Thermocron.Api;
using Thermocron.Data;

internal class Program
{
    private static string _token;
    private static readonly object _lock = new();
    private static IHttpClientFactory _httpClientFactory;
    private static string _mullerUsername;
    private static string _mullerPassword;
    private static string _mullerHomeId;
    private static string _connectionString;

    public static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appSettings.dev.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        _mullerUsername = config["thermocronMullerUsername"];
        _mullerPassword = config["thermocronMullerPassword"];
        _mullerHomeId = config["thermocronMullerHomeId"];
        _connectionString = config["connectionString"];

        if (string.IsNullOrEmpty(_mullerUsername) || string.IsNullOrEmpty(_mullerPassword) || string.IsNullOrEmpty(_mullerHomeId) || string.IsNullOrEmpty(_connectionString))
        {
            Console.WriteLine("Error: One or more required environment variables (thermocron_muller_username, thermocron_muller_password, thermocron_muller_home_id, connectionString) are missing.");
            Environment.Exit(1);
        }

        var services = new ServiceCollection();
        services.AddHttpClient();
        var serviceProvider = services.BuildServiceProvider();
        _httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

        _token = await GetPasswordTokenAsync();

        await Execute();

        while (true)
        {
            DateTime now = DateTime.Now;
            DateTime nextExecution = now.AddMinutes(5).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
            TimeSpan delay = nextExecution - now;

            try
            {
                var cancellationToken = new CancellationToken();
                await Task.Delay(delay, cancellationToken);
                _ = Task.Run(Execute, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private static async Task<string> GetPasswordTokenAsync()
    {
        var client = _httpClientFactory.CreateClient();

        var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = "https://app.muller-intuitiv.net/oauth2/token",
            ClientId = "59e604948fe283fd4dc7e355",
            ClientSecret = "rAeWu8Y3YqXEPqRJ4BpFzFG98MRXpCcz",
            Parameters = new Parameters(new []{ new KeyValuePair<string, string>("user_prefix", "muller") }),
            UserName = _mullerUsername,
            Password = _mullerPassword,
            Scope = "read_muller write_muller",
            GrantType = "password"
        });

        if (tokenResponse.IsError)
        {
            throw new Exception($"Token request failed: {tokenResponse.Error}");
        }

        return tokenResponse.AccessToken;
    }

    private static async Task Execute()
    {
        var netatmoClient = _httpClientFactory.CreateClient();
        netatmoClient.BaseAddress = new Uri("https://app.muller-intuitiv.net");
        lock (_lock)
        {
            netatmoClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        var response = await netatmoClient.PostAsync("syncapi/v1/homestatus", new FormUrlEncodedContent(new []{ new KeyValuePair<string, string>("home_id", _mullerHomeId) }));
        
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            Console.WriteLine("Token expired. Refreshing token...");
            lock (_lock)
            {
                _token = GetPasswordTokenAsync().Result;
                netatmoClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            }
            response = await netatmoClient.PostAsync("syncapi/v1/homestatus", new FormUrlEncodedContent(new []{ new KeyValuePair<string, string>("home_id", _mullerHomeId) }));
        }

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<Response>(json);

        if (data?.Body?.Home?.Rooms == null || data.Body.Home.Rooms.Count == 0)
        {
            Console.WriteLine($"Invalid JSON structure:\n{json}");
            return;
        }

        using var context = new AppDbContext(_connectionString);
        context.Database.EnsureCreated();

        var room = data.Body.Home.Rooms[0];
        var outdoorModule = data.Body.Home.Modules.FirstOrDefault(m => m.OutdoorTemperature.HasValue);

        if (outdoorModule == null)
        {
            Console.WriteLine("No valid outdoor module found.");
            return;
        }

        var roomId = Convert.ToInt32(room.Id);
        var device = context.Devices.FirstOrDefault(d => d.Id == roomId) ?? new Device { Id = roomId };

        if (context.Entry(device).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
        {
            context.Devices.Add(device);
            context.SaveChanges();
        }

        var measure = new Measure
        {
            DeviceId = device.Id,
            MeasuredTemperature = room.ThermMeasuredTemperature,
            TargetTemperature = room.ThermSetpointTemperature,
            OutdoorTemperature = outdoorModule.OutdoorTemperature ?? 0,
            Timestamp = DateTime.Now
        };

        context.Measures.Add(measure);
        context.SaveChanges();
    }
}
