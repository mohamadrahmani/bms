using BMS.Domain.Entities.BMS;
using BMS.Domain.Enums;

namespace BMS.Application.Points.Dtos;

public class CreatePointDto
{
    public Guid DeviceId { get; set; }


    public string Tag { get; set; } = default!;

    public PointKind Kind { get; set; }

    public ushort Address { get; set; }

    public string Title { get; set; } = default!;

    public PointDataType DataType { get; set; }

    public string? Unit { get; set; }
}
