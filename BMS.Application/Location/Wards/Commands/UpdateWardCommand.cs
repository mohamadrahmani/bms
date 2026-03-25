using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Location.Wards.Commands;

public class UpdateWardCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }

    public Guid FloorId { get; set; }

    public string Name { get; set; } = default!;
    public string? Type { get; set; }

    public string? Description { get; set; }
}
