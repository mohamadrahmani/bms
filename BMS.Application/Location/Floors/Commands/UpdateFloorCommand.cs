using BMS.Application.Attributes;
using BMS.Domain.Entities.Logs;
using MediatR;
using System.Text.Json.Serialization;

namespace BMS.Application.Location.Floors.Commands;
[Audit(EventType.UpdateData, "Floors")]
public class UpdateFloorCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid BuildingId { get; set; }
    public string Name { get; set; } = default!;

    public int LevelNumber { get; set; }

    public string? Description { get; set; }
    [JsonIgnore]
    public List<(string Field, string? OldValue, string? NewValue)> Changes { get; set; } = new();

}
