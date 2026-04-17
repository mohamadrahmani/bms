using BMS.Application.Attributes;
using BMS.Application.Models;
using BMS.Domain.Entities.Logs;
using MediatR;
using System.Text.Json.Serialization;

namespace BMS.Application.Location.Sites.Commands;

[Audit(EventType.UpdateData, "Sites")]
public sealed class UpdateSiteCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string? Address { get; set; }

    public string? Description { get; set; }

    [JsonIgnore]
    public List<(string Field, string? OldValue, string? NewValue)> Changes { get; set; } = new();
}
