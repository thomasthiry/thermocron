using Microsoft.EntityFrameworkCore;
using ThermocronApi.Data;
using ThermocronApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration de la base de données
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost:15432;Database=thermocron_dev;Username=thermocron_dev;Password=thermocron";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Enregistrement des services
builder.Services.AddScoped<TemperatureService>();

// Configuration CORS pour le frontend Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7000", "http://localhost:5000", "http://localhost:5100")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("BlazorPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
