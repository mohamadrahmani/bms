using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.Logs;
using MediatR;

namespace BMS.Application.Location.Buildings.Commands;
[Audit(EventType.AddData, "Buildings")]
public sealed class CreateBuildingCommand : IRequest<ApiResponse<Guid>>
{
    public Guid SiteId { get; set; }

    public string Name { get; set; } = default!;

    public string Code { get; set; } = default!;

    public string? Description { get; set; }
}
