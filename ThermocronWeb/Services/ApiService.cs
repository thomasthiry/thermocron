using System.Text.Json;
using ThermocronWeb.Models;

namespace ThermocronWeb.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient httpClient, ApiSettings apiSettings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(apiSettings.BaseUrl);
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<DeviceDto>> GetDevicesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("device");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<DeviceDto>>(json, _jsonOptions) ?? new List<DeviceDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la récupération des devices: {ex.Message}");
            return new List<DeviceDto>();
        }
    }

    public async Task<List<MeasureDto>> GetMeasuresAsync(int? deviceId, DateTime from, DateTime to, string interval = "hour")
    {
        try
        {
            var query = $"temperature/measures?from={from:yyyy-MM-ddTHH:mm:ss}&to={to:yyyy-MM-ddTHH:mm:ss}&interval={interval}";
            
            if (deviceId.HasValue)
            {
                query += $"&deviceId={deviceId.Value}";
            }

            var response = await _httpClient.GetAsync(query);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<MeasureDto>>(json, _jsonOptions) ?? new List<MeasureDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la récupération des mesures: {ex.Message}");
            return new List<MeasureDto>();
        }
    }

    public async Task<TemperatureStatsDto?> GetStatsAsync(int? deviceId, DateTime from, DateTime to)
    {
        try
        {
            var query = $"temperature/stats?from={from:yyyy-MM-ddTHH:mm:ss}&to={to:yyyy-MM-ddTHH:mm:ss}";
            
            if (deviceId.HasValue)
            {
                query += $"&deviceId={deviceId.Value}";
            }

            var response = await _httpClient.GetAsync(query);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TemperatureStatsDto>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la récupération des statistiques: {ex.Message}");
            return null;
        }
    }

    public async Task<MeasureDto?> GetLatestMeasureAsync(int? deviceId)
    {
        try
        {
            var query = "temperature/latest";
            
            if (deviceId.HasValue)
            {
                query += $"?deviceId={deviceId.Value}";
            }

            var response = await _httpClient.GetAsync(query);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MeasureDto>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la récupération de la dernière mesure: {ex.Message}");
            return null;
        }
    }
}
