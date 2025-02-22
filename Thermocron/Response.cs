using System.Text.Json.Serialization;

public class Response
{
    [JsonPropertyName("body")]
    public Body Body { get; set; }
}