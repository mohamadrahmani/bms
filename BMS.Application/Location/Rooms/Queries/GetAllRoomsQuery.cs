using BMS.Application.Common.Pagination;
using BMS.Application.Location.Rooms.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Location.Rooms.Queries
{
    public class GetAllRoomsQuery() : PagedRequest, IRequest<PagedResult<RoomDto>>
    {
    public Guid? FloorId { get; set; }

        public Guid? WardId { get; set; }

        public string? Search { get; set; }
    }
}
