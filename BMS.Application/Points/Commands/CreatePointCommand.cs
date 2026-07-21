using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Application.Points.Dtos;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Logs;
using MediatR;

namespace BMS.Application.Points.Commands;
[Audit(EventType.AddData, "Points")]
public class CreatePointCommand : IRequest<ApiResponse<Guid>>
{
    public Guid DeviceId { get; set; }


    public string? Tag { get; set; }

    public PointKind Kind { get; set; }

    public ushort Address { get; set; }

    public string Title { get; set; } = default!;

    public PointType PointType { get; set; }

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

    public ushort? FeedbackAddress { get; set; }

    public int ValidationRetryCount { get; set; } = 3;

    public int ValidationDelayMs { get; set; } = 200;

    public bool IsWritable { get; set; }

    public RegisterType? RegisterType { get; set; }

    public int? RegisterAddress { get; set; }

    public int? BitIndex { get; set; }

    public ByteOrder? ByteOrder { get; set; }
    public Guid? CommandDefinitionId { get; set; }
}
