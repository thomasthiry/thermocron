// See https://aka.ms/new-console-template for more information

using System.Net.Http.Headers;
using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;





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

// while (true)
// {
//     DateTime now = DateTime.Now;
//     DateTime nextExecution = now.AddMinutes(1).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
//     TimeSpan delay = nextExecution - now;
//
//     try
//     {
//         var cancellationToken = new CancellationToken();
//         await Task.Delay(delay, cancellationToken);
//         _ = Task.Run(Execute, cancellationToken);
//     }
//     catch (TaskCanceledException)
//     {
//         break; // Exit gracefully on cancellation
//     }
// }

async Task Execute()
{
    Console.WriteLine("Thermocron!");
    var response = await netatmoClient.PostAsync("syncapi/v1/homestatus", new FormUrlEncodedContent(new []{ new KeyValuePair<string, string> ( "home_id",  "" /* password manager */ )}));
    var json = await response.Content.ReadAsStringAsync();
    Console.WriteLine(json);
}
