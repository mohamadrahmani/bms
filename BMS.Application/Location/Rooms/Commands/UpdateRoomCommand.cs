using BMS.Application.Attributes;
using BMS.Domain.Entities.Logs;
using MediatR;
using System.Text.Json.Serialization;

//public record UpdateRoomCommand(
//    Guid Id,
//    Guid FloorId,
//    Guid WardId,
//    string Name,
//    string RoomNumber,
//    string? Type,
//    double Area
//) : IRequest;
[Audit(EventType.UpdateData, "Rooms")]
public class UpdateRoomCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid FloorId { get; set; }
    public Guid WardId { get; set; }
    public string Name { get; set; } = default!;

    public string RoomNumber { get; set; }
    public string? Type { get; set; }

    public double Area { get; set; }

    [JsonIgnore]
    public List<(string Field, string? OldValue, string? NewValue)> Changes { get; set; } = new();

}
