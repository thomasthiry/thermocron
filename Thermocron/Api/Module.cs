using System.Text.Json.Serialization;

namespace Thermocron.Api;

public class Module
{
    [JsonPropertyName("outdoor_temperature")]
    public double? OutdoorTemperature { get; set; }
}