using MediatR;
using BMS.Domain.Entities.Location;
using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Rooms.Commands;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, Guid>
{
    private readonly IRoomRepository _repository;
    private readonly IFloorRepository _floorRepository;
    private readonly IWardRepository _wardRepository;

    public CreateRoomCommandHandler(
        IRoomRepository repository,
        IFloorRepository floorRepository,
        IWardRepository wardRepository)
    {
        _repository = repository;
        _floorRepository = floorRepository;
        _wardRepository = wardRepository;
    }

    public async Task<Guid> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        if (!await _floorRepository.ExistsAsync(request.FloorId))
            throw new Exception("Floor not found");

        if (!await _wardRepository.ExistsAsync(request.WardId))
            throw new Exception("Ward not found");

        var room = new Room(
            request.FloorId,
            request.WardId,
            request.Name,
            request.RoomNumber,
            request.Type,
            request.Area
        );

        await _repository.AddAsync(room, cancellationToken);

        return room.Id;
    }
}
