using BMS.Application.Location.Rooms.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Location.Rooms.Queries
{

    public record GetRoomByIdQuery(Guid Id) : IRequest<RoomDto?>;
}
