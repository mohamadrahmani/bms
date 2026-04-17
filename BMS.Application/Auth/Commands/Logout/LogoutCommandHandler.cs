using MediatR;
using Microsoft.Extensions.Logging;
using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.Logs;

namespace BMS.Application.Auth.Commands.Logout;

public sealed class LogoutCommandHandler
    : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IAuditLogger _auditLogger;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IAuditLogger auditLogger,
        ILogger<LogoutCommandHandler> logger)
    {
        _auditLogger = auditLogger;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User {UserId} logged out",
            request.UserId);

        await _auditLogger.Add(new Log
        {
            UserId = request.UserId,
            EventType = EventType.Logout,
            Result = OperationResult.Success,
            ResultMessage = "خروج کاربر با موفقیت انجام شد",
            IpAddress = request.IpAddress,
            Source = nameof(LogoutCommandHandler),
            LogDate = DateTime.UtcNow,
            ObjectName = "Users",
            ObjectId = request.UserId.ToString()
        });

        return Unit.Value;
    }
}
