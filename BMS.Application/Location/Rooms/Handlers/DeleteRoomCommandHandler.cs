using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Rooms.Commands;

public class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand>
{
    private readonly IRoomRepository _repository;

    public DeleteRoomCommandHandler(IRoomRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (room == null)
            throw new Exception("Room not found");

        await _repository.DeleteAsync(room, cancellationToken);

        return Unit.Value;
    }
}
