using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Location.Wards.Commands;

public class CreateWardCommand : IRequest<ApiResponse<Guid>>
{
    public Guid FloorId { get; set; }

    public string Name { get; set; } = default!;
    public string? Type { get; set; }

    public string? Description { get; set; }
}
