using MediatR;

namespace BMS.Application.Location.Sites.Commands;

public sealed record CreateSiteCommand(
    string Name,
    string? Address,
    string? Description
) : IRequest<Guid>;
