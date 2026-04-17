using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.Logs;
using MediatR;

namespace BMS.Application.Location.Floors.Commands;
[Audit(EventType.DeleteData, "Floors")]
public class DeleteFloorCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}
