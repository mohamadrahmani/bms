namespace BMS.Application.Models;

public class PointValue
{
    public string Name { get; set; } = default!;
    public object? Value { get; set; }
    public DateTime Timestamp { get; set; }
}
