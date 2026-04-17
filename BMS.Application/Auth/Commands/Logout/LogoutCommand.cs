using MediatR;

namespace BMS.Application.Auth.Commands.Logout;

public sealed record LogoutCommand(
    Guid UserId,
    string IpAddress
) : IRequest<Unit>;
