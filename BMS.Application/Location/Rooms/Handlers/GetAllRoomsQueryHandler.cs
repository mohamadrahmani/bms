using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Pagination;
using BMS.Application.Location.Rooms.Dtos;
using BMS.Application.Location.Rooms.Queries;
using MediatR;

public class GetAllRoomsQueryHandler
    : IRequestHandler<GetAllRoomsQuery, PagedResult<RoomDto>>
{
    private readonly IRoomRepository _roomRepository;

    public GetAllRoomsQueryHandler(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<PagedResult<RoomDto>> Handle(GetAllRoomsQuery request, CancellationToken cancellationToken)
    {
        var rooms = _roomRepository.Rooms;

        var query = rooms.Select(r => new RoomDto
        {
            Id = r.Id,
            FloorId = r.FloorId,
            WardId = r.WardId,
            Name = r.Name,
            RoomNumber = r.RoomNumber,
            Type = r.Type,
            Area = r.Area
        });

        return await query.ToPagedResultAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

    }
}
