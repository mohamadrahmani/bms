using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Rooms.Commands;
using BMS.Application.Models;
using BMS.Domain.Entities.BMS;
using MediatR;

namespace BMS.Application.Rooms.Handlers;   

public class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand, ApiResponse<bool>>
{
    private readonly IRoomRepository _repository;

    public DeleteRoomCommandHandler(IRoomRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (room == null)
            throw new Exception("Room not found");

        await _repository.DeleteAsync(room, cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true, "کنترلر ایجاد شد");
    }
}
