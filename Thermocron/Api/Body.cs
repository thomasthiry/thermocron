using System.Text.Json.Serialization;

namespace Thermocron.Api;

public class Body
{
    [JsonPropertyName("home")]
    public Home Home { get; set; }
}