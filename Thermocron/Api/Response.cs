using System.Text.Json.Serialization;

namespace Thermocron.Api;

public class Response
{
    [JsonPropertyName("body")]
    public Body Body { get; set; }
}