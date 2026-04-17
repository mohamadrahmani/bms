using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.Logs;
using MediatR;

namespace BMS.Application.Points.Commands;
[Audit(EventType.DeleteData, "Points")]
public class DeletePointCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }

    public DeletePointCommand(Guid id)
    {
        Id = id;
    }
}
