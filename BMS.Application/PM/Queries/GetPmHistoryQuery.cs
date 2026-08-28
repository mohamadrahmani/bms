using BMS.Application.Pm.DTOs;
using MediatR;

namespace BMS.Application.Pm.Queries
{
    public record GetPmHistoryQuery(
        Guid DeviceId,
        string Tag
    ) : IRequest<List<PmHistoryDto>>;
}