using BMS.Domain.Entities.BMS;
using BMS.Domain.Enums;

namespace BMS.Application.Points.Dtos;

public class PointDto
{
    public Guid Id { get; set; }

    public Guid DeviceId { get; set; }

    public string Tag { get; set; } = default!;

    public string Title { get; set; } = default!;

    public PointKind Kind { get; set; }

    public PointDataType DataType { get; set; }

    public ushort? Address { get; set; }

    public string? Unit { get; set; }

    public string? Code { get; set; }

    public ushort Length { get; set; }

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
}
