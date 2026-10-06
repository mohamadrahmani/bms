using MediatR;
using Microsoft.Extensions.Logging;
using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.Logs;

namespace BMS.Application.Auth.Commands.Logout;

public sealed class LogoutCommandHandler
    : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IAuditLogger _auditLogger;
    private readonly ILogRepository _logRepository;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IAuditLogger auditLogger,
        ILogRepository logRepository,
        ILogger<LogoutCommandHandler> logger)
    {
        _auditLogger = auditLogger;
        _logRepository = logRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User {UserId} logged out",
            request.UserId);

        var userName = await _logRepository.GetObjectDisplayNameAsync("Users", request.UserId, cancellationToken);

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
            ObjectId = request.UserId.ToString(),
            ObjectDisplayName = userName
        });

        return Unit.Value;
    }
}
