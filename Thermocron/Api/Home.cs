using System.Text.Json.Serialization;

namespace Thermocron.Api;

public class Home
{
    [JsonPropertyName("modules")]
    public List<Module> Modules { get; set; }

    [JsonPropertyName("rooms")]
    public List<Room> Rooms { get; set; }
}