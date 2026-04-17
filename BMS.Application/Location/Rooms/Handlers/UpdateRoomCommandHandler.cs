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
        // Track Changes
        TrackChange(request, "FloorId", room.FloorId.ToString(), request.FloorId.ToString());
        TrackChange(request, "WardId", room.WardId.ToString(), request.WardId.ToString());
        TrackChange(request, "Name", room.Name, request.Name);
        TrackChange(request, "RoomNumber", room.RoomNumber, request.RoomNumber);
        TrackChange(request, "Type", room.Type, request.Type);
        TrackChange(request, "Area", room.Area.ToString(), request.Area.ToString());

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
    private static void TrackChange(UpdateRoomCommand request, string field, string? oldValue, string? newValue)
    {
        if (oldValue != newValue)
        {
            request.Changes.Add((field, oldValue, newValue));
        }
    }
}
