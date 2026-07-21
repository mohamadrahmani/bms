using BMS.Domain.Entities.BMS;
using BMS.Domain.Enums;

namespace BMS.Application.Points.Dtos;

public class PointDto
{
    public Guid Id { get; set; }

    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; }

    public string? Tag { get; set; }

    public string Title { get; set; } = default!;

    public PointKind Kind { get; set; }

    public PointDataType DataType { get; set; }
    public PointType PointType { get; set; }
    public string PointTypeTitle
    { 
        get
        {
            return PointType.ToString();
        }
    }
    public ushort? Address { get; set; }

    public ushort? FeedbackAddress { get; set; }
    public int ValidationRetryCount { get; set; }
    public int ValidationDelayMs { get; set; }
    public string? Unit { get; set; }

    public string? Code { get; set; }

    public int Length { get; set; }

    public double Scale { get; set; }

    public double Offset { get; set; }

    public bool IsWritable { get; set; }

    public RegisterType? RegisterType { get; set; }

    public int? RegisterAddress { get; set; }

    public int? BitIndex { get; set; }

    public ByteOrder? ByteOrder { get; set; }

    public Guid? SiteId { get; set; }
    public Guid? BuildingId { get; set; }
    public Guid? FloorId { get; set; }
    public Guid? WardId { get; set; }
    public Guid? RoomId { get; set; }

    public string? Value { get; set; }

    public string? Quality { get; set; }

    public DateTime? LastUpdatedAtUtc { get; set; }
    public Guid ControllerId { get; set; }
    public string ControllerCode { get; set; }
    public string ControllerName { get; set; }
    public string IpAddress { get; set; }

    public string DeviceCode { get; set; }
    public Guid? CommandDefinitionId { get; set; }

}
