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

    public async Task<ApiResult<List<DeviceDto>>> GetDevicesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("device");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResult<List<DeviceDto>>.Error(
                    $"Erreur lors de la récupération des capteurs: {errorContent}", 
                    (int)response.StatusCode);
            }
            
            var json = await response.Content.ReadAsStringAsync();
            var devices = JsonSerializer.Deserialize<List<DeviceDto>>(json, _jsonOptions) ?? new List<DeviceDto>();
            return ApiResult<List<DeviceDto>>.Success(devices);
        }
        catch (Exception ex)
        {
            return ApiResult<List<DeviceDto>>.Error(
                $"Erreur de connexion lors de la récupération des capteurs: {ex.Message}", 
                500);
        }
    }

    public async Task<ApiResult<List<MeasureDto>>> GetMeasuresAsync(int? deviceId, DateTime from, DateTime to, string interval = "hour")
    {
        try
        {
            var query = $"temperature/measures?from={from:yyyy-MM-ddTHH:mm:ss}&to={to:yyyy-MM-ddTHH:mm:ss}&interval={interval}";
            
            if (deviceId.HasValue)
            {
                query += $"&deviceId={deviceId.Value}";
            }

            var response = await _httpClient.GetAsync(query);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResult<List<MeasureDto>>.Error(
                    $"Erreur lors de la récupération des mesures: {errorContent}", 
                    (int)response.StatusCode);
            }
            
            var json = await response.Content.ReadAsStringAsync();
            var measures = JsonSerializer.Deserialize<List<MeasureDto>>(json, _jsonOptions) ?? new List<MeasureDto>();
            return ApiResult<List<MeasureDto>>.Success(measures);
        }
        catch (Exception ex)
        {
            return ApiResult<List<MeasureDto>>.Error(
                $"Erreur de connexion lors de la récupération des mesures: {ex.Message}", 
                500);
        }
    }

    public async Task<ApiResult<TemperatureStatsDto>> GetStatsAsync(int? deviceId, DateTime from, DateTime to)
    {
        try
        {
            var query = $"temperature/stats?from={from:yyyy-MM-ddTHH:mm:ss}&to={to:yyyy-MM-ddTHH:mm:ss}";
            
            if (deviceId.HasValue)
            {
                query += $"&deviceId={deviceId.Value}";
            }

            var response = await _httpClient.GetAsync(query);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResult<TemperatureStatsDto>.Error(
                    $"Erreur lors de la récupération des statistiques: {errorContent}", 
                    (int)response.StatusCode);
            }
            
            var json = await response.Content.ReadAsStringAsync();
            var stats = JsonSerializer.Deserialize<TemperatureStatsDto>(json, _jsonOptions);
            return ApiResult<TemperatureStatsDto>.Success(stats!);
        }
        catch (Exception ex)
        {
            return ApiResult<TemperatureStatsDto>.Error(
                $"Erreur de connexion lors de la récupération des statistiques: {ex.Message}", 
                500);
        }
    }

    public async Task<ApiResult<MeasureDto>> GetLatestMeasureAsync(int? deviceId)
    {
        try
        {
            var query = "temperature/latest";
            
            if (deviceId.HasValue)
            {
                query += $"?deviceId={deviceId.Value}";
            }

            var response = await _httpClient.GetAsync(query);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResult<MeasureDto>.Error(
                    $"Erreur lors de la récupération de la dernière mesure: {errorContent}", 
                    (int)response.StatusCode);
            }
            
            var json = await response.Content.ReadAsStringAsync();
            var measure = JsonSerializer.Deserialize<MeasureDto>(json, _jsonOptions);
            return ApiResult<MeasureDto>.Success(measure!);
        }
        catch (Exception ex)
        {
            return ApiResult<MeasureDto>.Error(
                $"Erreur de connexion lors de la récupération de la dernière mesure: {ex.Message}", 
                500);
        }
    }
}
