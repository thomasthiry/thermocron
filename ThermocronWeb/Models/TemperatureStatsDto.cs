namespace ThermocronWeb.Models;

public class TemperatureStatsDto
{
    public double MinMeasured { get; set; }
    public double MaxMeasured { get; set; }
    public double AvgMeasured { get; set; }
    public double MinTarget { get; set; }
    public double MaxTarget { get; set; }
    public double AvgTarget { get; set; }
    public double MinOutdoor { get; set; }
    public double MaxOutdoor { get; set; }
    public double AvgOutdoor { get; set; }
    public int TotalMeasures { get; set; }
}
