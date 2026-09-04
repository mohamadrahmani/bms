using BMS.Application.Common.Interfaces;
using BMS.Application.SystemErrorLogs.Models;
using BMS.Application.SystemErrorLogs.Queries;
using MediatR;

namespace BMS.Application.SystemErrorLogs.Handlers;

public sealed class GetSystemErrorLogByIdHandler
    : IRequestHandler<GetSystemErrorLogByIdQuery, SystemErrorLogDetailsDto?>
{
    private readonly ISystemErrorLogRepository _repository;

    public GetSystemErrorLogByIdHandler(ISystemErrorLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<SystemErrorLogDetailsDto?> Handle(
        GetSystemErrorLogByIdQuery request,
        CancellationToken cancellationToken)
    {
        var log = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (log is null)
            return null;

        return new SystemErrorLogDetailsDto
        {
            Id = log.Id,
            ErrorId = log.ErrorId,
            OccurredAtUtc = log.OccurredAtUtc,
            ExceptionType = log.ExceptionType,
            Message = log.Message,
            StackTrace = log.StackTrace,
            InnerException = log.InnerException,
            RequestPath = log.RequestPath,
            HttpMethod = log.HttpMethod,
            QueryString = log.QueryString,
            RequestBody = log.RequestBody,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent,
            StatusCode = log.StatusCode,
            UserId = log.UserId,
            UserName = log.UserName,
            EnvironmentName = log.EnvironmentName,
            IsResolved = log.IsResolved,
            ResolvedAtUtc = log.ResolvedAtUtc,
            ResolvedByUserId = log.ResolvedByUserId,
            ResolutionNote = log.ResolutionNote
        };
    }
}
