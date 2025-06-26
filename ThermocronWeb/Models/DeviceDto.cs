namespace ThermocronWeb.Models;

public class DeviceDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime? LastMeasureTime { get; set; }
    public double? LastMeasuredTemperature { get; set; }
}
