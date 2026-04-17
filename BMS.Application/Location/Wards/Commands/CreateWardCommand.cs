using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.Logs;
using MediatR;

namespace BMS.Application.Location.Wards.Commands;
[Audit(EventType.AddData, "Wards")]
public class CreateWardCommand : IRequest<ApiResponse<Guid>>
{
    public Guid FloorId { get; set; }

    public string Name { get; set; } = default!;
    public string? Type { get; set; }

    public string? Description { get; set; }
}
