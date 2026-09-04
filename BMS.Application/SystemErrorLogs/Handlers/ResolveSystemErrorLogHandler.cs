using BMS.Application.Common.Interfaces;
using BMS.Application.SystemErrorLogs.Commands;
using MediatR;

namespace BMS.Application.SystemErrorLogs.Handlers;

public sealed class ResolveSystemErrorLogHandler
    : IRequestHandler<ResolveSystemErrorLogCommand, bool>
{
    private readonly ISystemErrorLogRepository _repository;

    public ResolveSystemErrorLogHandler(ISystemErrorLogRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(
        ResolveSystemErrorLogCommand request,
        CancellationToken cancellationToken)
    {
        return _repository.ResolveAsync(
            request.Id,
            request.ResolvedByUserId,
            request.ResolutionNote,
            cancellationToken);
    }
}
