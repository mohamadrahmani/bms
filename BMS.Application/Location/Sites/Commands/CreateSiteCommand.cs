using BMS.Application.Attributes;
using BMS.Domain.Entities.Logs;
using MediatR;

namespace BMS.Application.Location.Sites.Commands;

[Audit(EventType.AddData, "Sites")]
public sealed class CreateSiteCommand : IRequest<Guid>
{
    public string Name { get; set; } = default!;

    public string? Address { get; set; }

    public string? Description { get; set; }
}
