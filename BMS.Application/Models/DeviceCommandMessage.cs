namespace BMS.Application.Models;

public class DeviceCommandMessagex
{
    public Guid ControllerId { get; set; }

    public Guid DeviceId { get; set; }

    public Guid PointId { get; set; }

    public object Value { get; set; }

    public DateTime Timestamp { get; set; }
}
