namespace BMS.Application.Auth.Commands.Login;

public sealed record LoginResponse(
    Guid UserId,
    string UserName,
    string Token,
    DateTime ExpiresAt
);
