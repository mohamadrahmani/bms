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

        if (request.FloorId.HasValue)
        {
            rooms = rooms.Where(r => r.FloorId == request.FloorId.Value);
        }

        if (request.WardId.HasValue)
        {
            rooms = rooms.Where(r => r.WardId == request.WardId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            rooms = rooms.Where(r =>
                r.Name.Contains(request.Search) ||
                r.RoomNumber.Contains(request.Search));
        }


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
