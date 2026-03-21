using MediatR;

namespace BMS.Application.Location.Buildings.Commands;

public sealed class CreateBuildingCommand : IRequest<Guid>
{
    public Guid SiteId { get; set; }

    public string Name { get; set; } = default!;

    public string Code { get; set; } = default!;

    public string? Description { get; set; }
}
