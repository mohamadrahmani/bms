namespace BMS.Application.Models;

public class PlcSnapshot
{
    public string PlcName { get; set; } = default!;
    public DateTime Timestamp { get; set; }
    public List<PointValue> Points { get; set; } = new();
}
