using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Location.Floors.Commands;

public class CreateFloorCommand : IRequest<ApiResponse<Guid>>
{
    public Guid BuildingId { get; set; }

    public string Name { get; set; } = default!;

    public int LevelNumber { get; set; }

    public string? Description { get; set; }
}
