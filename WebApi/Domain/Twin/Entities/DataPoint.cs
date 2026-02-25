namespace WebApi.Domain.Twin.Entities;

public class DataPoint
{
    public string Id { get; private set; }

    public object? Value { get; private set; }

    public DateTime Timestamp { get; private set; }

    public DataPoint(string id, object? initialValue = null)
    {
        Id = id;
        Value = initialValue;
        Timestamp = DateTime.UtcNow;
    }

    public bool Update(object? newValue)
    {
        if (Equals(Value, newValue))
            return false;

        Value = newValue;
        Timestamp = DateTime.UtcNow;
        return true;
    }
}
