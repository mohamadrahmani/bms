using BMS.Application.Attributes;
using BMS.Domain.Entities.Logs;
using MediatR;
using System.Text.Json.Serialization;

namespace BMS.Application.Location.Wards.Commands;
[Audit(EventType.UpdateData, "Wards")]
public class UpdateWardCommand : IRequest
{
    public Guid Id { get; set; }

    public Guid FloorId { get; set; }

    public string Name { get; set; } = default!;
    public string? Type { get; set; }

    public string? Description { get; set; }
    [JsonIgnore]
    public List<(string Field, string? OldValue, string? NewValue)> Changes { get; set; } = new();

}
