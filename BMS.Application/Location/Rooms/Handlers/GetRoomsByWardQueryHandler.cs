using BMS.Application.Location.Rooms.Dtos;
using BMS.Application.Location.Rooms.Queries;
using BMS.Application.Common.Interfaces;
using MediatR;

namespace BMS.Application.Location.Rooms.Handlers
{
    public class GetRoomsByWardQueryHandler
        : IRequestHandler<GetRoomsByWardQuery, List<RoomDto>>
    {
        private readonly IRoomRepository _roomRepository;

        public GetRoomsByWardQueryHandler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<List<RoomDto>> Handle(GetRoomsByWardQuery request, CancellationToken cancellationToken)
        {
            var rooms = await _roomRepository.GetByWardIdAsync(request.WardId, cancellationToken);

            return rooms.Select(room => new RoomDto
            {
                Id = room.Id,
                FloorId = room.FloorId,
                WardId = room.WardId,
                Name = room.Name,
                RoomNumber = room.RoomNumber,
                Type = room.Type,
                Area = room.Area
            }).ToList();
        }
    }
}
