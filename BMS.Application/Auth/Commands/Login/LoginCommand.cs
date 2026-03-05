using MediatR;

namespace BMS.Application.Auth.Commands.Login;

public sealed record LoginCommand(
    string UserName,
    string Password
) : IRequest<LoginResponse>;
