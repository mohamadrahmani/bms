using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.Logs;
using MediatR;

namespace BMS.Application.Location.Buildings.Commands;
[Audit(EventType.DeleteData, "Buildings")]
public sealed class DeleteBuildingCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
    public Guid SiteId { get; set; }
}
