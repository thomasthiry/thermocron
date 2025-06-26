using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ThermocronWeb;
using ThermocronWeb.Services;
using ApexCharts;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration du HttpClient pour l'API
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped(sp => new HttpClient());

// Ajout d'ApexCharts
builder.Services.AddApexCharts();

await builder.Build().RunAsync();
