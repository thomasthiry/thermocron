// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Duende.AccessTokenManagement;
using Duende.IdentityModel.Client;
using Microsoft.Extensions.Logging;

var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging(builder => builder.AddConsole());
serviceCollection.AddDistributedMemoryCache();
serviceCollection.AddClientCredentialsHttpClient("NetatmoApi", "NetatmoApi.client", client =>
{
    client.BaseAddress = new Uri("https://app.muller-intuitiv.net");
});

serviceCollection.AddClientCredentialsTokenManagement()
    .AddClient("NetatmoApi.client", client =>
    {
        client.TokenEndpoint = "https://app.muller-intuitiv.net/oauth2/token";

        client.ClientId = "59e604948fe283fd4dc7e355";
        client.ClientSecret = "rAeWu8Y3YqXEPqRJ4BpFzFG98MRXpCcz";

        client.Scope = "read_muller write_muller";
        
        client.Parameters.Add("user_prefix", "muller", ParameterReplaceBehavior.All);
        client.Parameters.Add("grant_type", "password", ParameterReplaceBehavior.All);
        client.Parameters.Add("username", "", ParameterReplaceBehavior.All); // password manager
        client.Parameters.Add("password", "", ParameterReplaceBehavior.All); // Password manager
    });

// Step 2: Build the service provider
var serviceProvider = serviceCollection.BuildServiceProvider();

// Step 3: Use the service
var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

var netatmoClient = httpClientFactory.CreateClient("NetatmoApi");

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
