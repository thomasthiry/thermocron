namespace ThermocronApi.DTOs;

public class MeasureDto
{
    public DateTime Timestamp { get; set; }
    public double MeasuredTemperature { get; set; }
    public double TargetTemperature { get; set; }
    public double OutdoorTemperature { get; set; }
}
