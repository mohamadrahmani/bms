namespace BMS.Application.Models;

public class DeviceSnapshotDto
{
    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; } = default!;
    public DateTime Timestamp { get; set; }
    public List<SensorValueDto> Sensors { get; set; } = new();
}

public class SensorValueDto
{
    public Guid SensorId { get; set; }
    public double Value { get; set; }
    public string? Name { get; set; }
    public ushort Address { get; set; }
}
