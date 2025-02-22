using System.Text.Json.Serialization;

public class Body
{
    [JsonPropertyName("home")]
    public Home Home { get; set; }
}