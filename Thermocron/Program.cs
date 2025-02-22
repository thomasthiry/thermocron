// See https://aka.ms/new-console-template for more information

using System.Net.Http.Headers;
using System.Text.Json;
using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Thermocron.Api;
using Thermocron.Data;


var services = new ServiceCollection();
services.AddHttpClient();

var serviceProvider = services.BuildServiceProvider();
var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

async Task<string> GetPasswordTokenAsync()
{
    var client = httpClientFactory.CreateClient();

    var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
    {
        Address = "https://app.muller-intuitiv.net/oauth2/token",
        ClientId = "59e604948fe283fd4dc7e355",
        ClientSecret = "rAeWu8Y3YqXEPqRJ4BpFzFG98MRXpCcz",
        Parameters = new Parameters(new []{ new KeyValuePair<string, string>("user_prefix", "muller") }),
        UserName = "",/* password manager */
        Password = "",/* password manager */
        Scope = "read_muller write_muller",
        GrantType = "password"
    });

    if (tokenResponse.IsError)
    {
        throw new Exception($"Token request failed: {tokenResponse.Error}");
    }

    return tokenResponse.AccessToken;
}

var token = await GetPasswordTokenAsync();

var netatmoClient = httpClientFactory.CreateClient("NetatmoApi");
netatmoClient.BaseAddress = new Uri("https://app.muller-intuitiv.net");
netatmoClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

await Execute();

while (true)
{
    DateTime now = DateTime.Now;
    DateTime nextExecution = now.AddMinutes(1).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
    TimeSpan delay = nextExecution - now;

    try
    {
        var cancellationToken = new CancellationToken();
        await Task.Delay(delay, cancellationToken);
        _ = Task.Run(Execute, cancellationToken);
    }
    catch (TaskCanceledException)
    {
        break; // Exit gracefully on cancellation
    }
}

async Task Execute()
{
    var response = await netatmoClient.PostAsync("syncapi/v1/homestatus", new FormUrlEncodedContent(new []{ new KeyValuePair<string, string> ( "home_id",  "" /* password manager */ )}));
    var json = await response.Content.ReadAsStringAsync();
    
    var data = JsonSerializer.Deserialize<Response>(json);

    if (data?.Body?.Home?.Rooms == null || data.Body.Home.Rooms.Count == 0)
    {
        Console.WriteLine("Invalid JSON structure.");
        return;
    }

    using var context = new AppDbContext();

    context.Database.EnsureCreated();

    // Extract room temperature data
    var room = data.Body.Home.Rooms[0];
    var outdoorModule = data.Body.Home.Modules.FirstOrDefault(m => m.OutdoorTemperature.HasValue);

    if (outdoorModule == null)
    {
        Console.WriteLine("No valid outdoor module found.");
        return;
    }

    var roomId = Convert.ToInt32(room.Id);
    var device = context.Devices.FirstOrDefault(d => d.Id == roomId);

    if (device == null)
    {
        device = new Device { Id = roomId };

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