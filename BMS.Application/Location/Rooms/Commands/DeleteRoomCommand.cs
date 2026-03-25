using BMS.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Location.Rooms.Commands
{
    public record DeleteRoomCommand(Guid Id) : IRequest<ApiResponse<bool>>;

}
