using System.Text.Json.Serialization;

public class Room
{
    [JsonPropertyName("therm_measured_temperature")]
    public double ThermMeasuredTemperature { get; set; }

    [JsonPropertyName("therm_setpoint_temperature")]
    public double ThermSetpointTemperature { get; set; }
}