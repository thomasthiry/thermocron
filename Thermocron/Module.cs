using System.Text.Json.Serialization;

public class Module
{
    [JsonPropertyName("outdoor_temperature")]
    public double? OutdoorTemperature { get; set; }
}