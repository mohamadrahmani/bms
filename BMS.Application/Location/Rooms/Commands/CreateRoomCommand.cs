using MediatR;
using BMS.Application.Location.Rooms.Dtos;

public record CreateRoomCommand(
    Guid FloorId,
    Guid WardId,
    string Name,
    string RoomNumber,
    string? Type,
    double Area
) : IRequest<Guid>;
