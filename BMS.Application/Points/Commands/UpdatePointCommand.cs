
using BMS.Application.Attributes;
using BMS.Application.Devices.DTOs;
using BMS.Application.Models;
using BMS.Application.Points.Dtos;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Entities.Logs;
using BMS.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace BMS.Application.Points.Commands;
[Audit(EventType.UpdateData, "Points")]
public class UpdatePointCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }

    public string? Tag { get; set; }
    public string Title { get; set; } = default!;

    public PointKind Kind { get; set; }
    public PointType PointType { get; set; }
    public PointDataType DataType { get; set; }

    public ushort Address { get; set; }
    public string? Unit { get; set; }

    public string? Code { get; set; }
    public ushort Length { get; set; }
    public double Scale { get; set; }
    public double Offset { get; set; }

    public ushort? FeedbackAddress { get; set; }

    public int ValidationRetryCount { get; set; }
    public int ValidationDelayMs { get; set; }

    public bool IsWritable { get; set; }

    public bool? StoreHistory { get; set; }

    public RegisterType RegisterType { get; set; }
    public ushort? RegisterAddress { get; set; }
    public int? BitIndex { get; set; }
    public ByteOrder? ByteOrder { get; set; }
    public Guid? CommandDefinitionId { get; set; }
    public Guid? SiteId { get; set; }
    public Guid? BuildingId { get; set; }
    public Guid? FloorId { get; set; }
    public Guid? WardId { get; set; }
    public Guid? RoomId { get; set; }
    [JsonIgnore]
    public List<(string Field, string? OldValue, string? NewValue)> Changes { get; set; } = new();
}
