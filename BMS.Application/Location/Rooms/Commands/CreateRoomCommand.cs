using BMS.Application.Location.Rooms.Dtos;
using BMS.Application.Models;
using MediatR;

public record CreateRoomCommand(
    Guid FloorId,
    Guid WardId,
    string Name,
    string RoomNumber,
    string? Type,
    double Area
) : IRequest<ApiResponse<Guid>>;
