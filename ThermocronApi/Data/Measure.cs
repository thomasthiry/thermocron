using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThermocronApi.Data;

public class Measure
{
    [Key]
    public int Id { get; set; }

    public double MeasuredTemperature { get; set; }

    public double TargetTemperature { get; set; }

    public double OutdoorTemperature { get; set; }

    public DateTime Timestamp { get; set; }

    [ForeignKey("Device")]
    public int DeviceId { get; set; }

    public Device Device { get; set; }
}
