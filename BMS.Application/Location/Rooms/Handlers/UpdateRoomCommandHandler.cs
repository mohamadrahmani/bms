using MediatR;
using BMS.Application.Common.Interfaces;

public class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand>
{
    private readonly IRoomRepository _repository;
    private readonly IFloorRepository _floorRepository;
    private readonly IWardRepository _wardRepository;

    public UpdateRoomCommandHandler(
        IRoomRepository repository,
        IFloorRepository floorRepository,
        IWardRepository wardRepository)
    {
        _repository = repository;
        _floorRepository = floorRepository;
        _wardRepository = wardRepository;
    }

    public async Task<Unit> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (room == null)
            throw new Exception("Room not found");

        if (!await _floorRepository.ExistsAsync(request.FloorId))
            throw new Exception("Floor not found");

        if (!await _wardRepository.ExistsAsync(request.WardId))
            throw new Exception("Ward not found");

        room.Update(
            request.FloorId,
            request.WardId,
            request.Name,
            request.RoomNumber,
            request.Type,
            request.Area
        );

        await _repository.UpdateAsync(room, cancellationToken);

        return Unit.Value;
    }
}
