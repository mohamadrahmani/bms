using MediatR;

namespace BMS.Application.SystemErrorLogs.Commands;

public sealed record ResolveSystemErrorLogCommand(
    Guid Id,
    Guid? ResolvedByUserId,
    string? ResolutionNote) : IRequest<bool>;
