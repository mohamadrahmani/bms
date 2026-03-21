using MediatR;

public record UpdateRoomCommand(
    Guid Id,
    Guid FloorId,
    Guid WardId,
    string Name,
    string RoomNumber,
    string? Type,
    double Area
) : IRequest;
