using BMS.Application.Points.Dtos;
using BMS.Domain.Entities.BMS;
using MediatR;

namespace BMS.Application.Points.Commands;

public class CreatePointCommand : IRequest<Guid>
{
    public Guid DeviceId { get; set; }


    public string Tag { get; set; } = default!;

    public PointKind Kind { get; set; }

    public ushort Address { get; set; }

    public string Title { get; set; } = default!;

    public PointDataType DataType { get; set; }

    public string? Unit { get; set; }
    public Guid? SiteId { get; init; }
    public Guid? BuildingId { get; init; }
    public Guid? FloorId { get; init; }
    public Guid? WardId { get; init; }
    public Guid? RoomId { get; init; }
    public string? Code { get; set; }

    public ushort Length { get; set; } = 1;

    public double Scale { get; set; } = 1;

    public double Offset { get; set; } = 0;

    public ushort? CommandAddress { get; set; }

    public ushort? FeedbackAddress { get; set; }

    public int ValidationRetryCount { get; set; } = 3;

    public int ValidationDelayMs { get; set; } = 200;

    public bool IsWritable { get; set; }

    public RegisterType? RegisterType { get; set; }

    public int? RegisterAddress { get; set; }

    public int? BitIndex { get; set; }

    public ByteOrder? ByteOrder { get; set; }
}
