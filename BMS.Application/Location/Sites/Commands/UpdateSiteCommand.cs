using BMS.Application.Models;
using MediatR;

namespace BMS.Application.Location.Sites.Commands;

public sealed record UpdateSiteCommand(
    Guid Id,
    string Name,
    string? Address,
    string? Description
) : IRequest<ApiResponse<bool>>;
