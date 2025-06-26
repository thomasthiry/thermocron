using System.ComponentModel.DataAnnotations;

namespace ThermocronApi.Data;

public class Device
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    public ICollection<Measure> Measures { get; set; } = new List<Measure>();
}
