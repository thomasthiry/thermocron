using System.Text.Json.Serialization;

namespace Thermocron.Api;

public class Room
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("therm_measured_temperature")]
    public double ThermMeasuredTemperature { get; set; }

    [JsonPropertyName("therm_setpoint_temperature")]
    public double ThermSetpointTemperature { get; set; }
}