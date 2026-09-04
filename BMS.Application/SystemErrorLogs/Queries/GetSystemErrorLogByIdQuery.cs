using BMS.Application.SystemErrorLogs.Models;
using MediatR;

namespace BMS.Application.SystemErrorLogs.Queries;

public sealed record GetSystemErrorLogByIdQuery(Guid Id)
    : IRequest<SystemErrorLogDetailsDto?>;
