using MediatR;
using BMS.Application.Location.Rooms.Dtos;
using BMS.Application.Attributes;
using BMS.Domain.Entities.Logs;

namespace BMS.Application.Location.Rooms.Commands;

[Audit(EventType.AddData, "Rooms")]
public class CreateRoomCommand : IRequest<Guid>
{
    public Guid FloorId { get; set; }
    public Guid WardId { get; set; }
    public string Name { get; set; } = default!;
    public string RoomNumber { get; set; } = default!;
    public string? Type { get; set; } = default!;
    public double Area { get; set; }
}
