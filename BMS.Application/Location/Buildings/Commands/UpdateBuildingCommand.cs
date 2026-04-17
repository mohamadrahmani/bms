using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.Logs;
using MediatR;
using System.Text.Json.Serialization;

namespace BMS.Application.Location.Buildings.Commands;
[Audit(EventType.UpdateData, "Buildings")]
public sealed class UpdateBuildingCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
    public Guid SiteId { get; set; }

    public string Name { get; set; } = default!;

    public string Code { get; set; } = default!;

    public string? Description { get; set; }
    [JsonIgnore]
    public List<(string Field, string? OldValue, string? NewValue)> Changes { get; set; } = new();

}
