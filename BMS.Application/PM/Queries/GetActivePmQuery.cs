using BMS.Application.Pm.DTOs;
using MediatR;

namespace BMS.Application.Pm.Queries
{
    public record GetActivePmQuery(
        Guid DeviceId,
        string? Tag
    ) : IRequest<List<PmDto>?>;
}