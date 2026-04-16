namespace BMS.Application.Models;

public class PointValue
{
    public PointValue()
    {

    }
    public PointValue( string name, object value)
    {
        Name = name;
        Value = value;
    }
    public string Name { get; set; } = default!;
    public object? Value { get; set; }
    public DateTime Timestamp { get; set; }
}
