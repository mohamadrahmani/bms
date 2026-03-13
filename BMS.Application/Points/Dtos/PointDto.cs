using BMS.Domain.Entities.BMS;
using BMS.Domain.Enums;

namespace BMS.Application.Points.Dtos;

public class PointDto
{
    public Guid Id { get; set; }

    public Guid DeviceId { get; set; }

    public string Title { get; set; } = default!;

    public PointKind Kind { get; set; }

    public PointDataType DataType { get; set; }

    public ushort? Address { get; set; }

    public string? Unit { get; set; }

    public string? Value { get; set; }

    public string? Quality { get; set; }

    public DateTime? LastUpdatedAtUtc { get; set; }
}
