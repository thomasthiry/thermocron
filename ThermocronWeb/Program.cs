using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ThermocronWeb;
using ThermocronWeb.Services;
using ThermocronWeb.Models;
using ApexCharts;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


// var apiSettings = new ApiSettings { BaseUrl = "http://localhost:8080/api/" };
var apiSettings = new ApiSettings { BaseUrl = "https://thermocron-api.evolve11.com/api/" };

builder.Services.AddSingleton(apiSettings);

// Configuration du HttpClient pour l'API
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped(sp => new HttpClient());

// Ajout d'ApexCharts
builder.Services.AddApexCharts();

await builder.Build().RunAsync();
