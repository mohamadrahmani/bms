using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Location.Buildings.Commands;

public sealed class UpdateBuildingCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
    public Guid SiteId { get; set; }

    public string Name { get; set; } = default!;

    public string Code { get; set; } = default!;

    public string? Description { get; set; }
}
