using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.Logs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Location.Rooms.Commands
{
    //    public record DeleteRoomCommand(Guid Id) : IRequest;

    //}
    [Audit(EventType.DeleteData, "Rooms")]
    public class DeleteRoomCommand : IRequest<ApiResponse<bool>>
    {
        public Guid Id { get; set; }
        public DeleteRoomCommand(Guid id)
        {
            Id = id;
        }
    }
}
