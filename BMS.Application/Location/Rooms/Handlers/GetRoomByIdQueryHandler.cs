using BMS.Application.Location.Rooms.Dtos;
using BMS.Application.Location.Rooms.Queries;
using BMS.Application.Common.Interfaces;
using MediatR;

namespace BMS.Application.Location.Rooms.Handlers
{
    public class GetRoomByIdQueryHandler
        : IRequestHandler<GetRoomByIdQuery, RoomDto?>
    {
        private readonly IRoomRepository _roomRepository;

        public GetRoomByIdQueryHandler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<RoomDto?> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
        {
            var room = await _roomRepository.GetByIdAsync(request.Id, cancellationToken);
            if (room == null)
                return null;

            return new RoomDto
            {
                Id = room.Id,
                FloorId = room.FloorId,
                WardId = room.WardId,
                Name = room.Name,
                RoomNumber = room.RoomNumber,
                Type = room.Type,
                Area = room.Area
            };
        }
    }
}
