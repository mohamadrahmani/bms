using BMS.Application.Common.Interfaces;
using BMS.Application.Pm.DTOs;
using BMS.Application.Pm.Queries;
using MediatR;

namespace BMS.Application.Pm.QueryHandlers
{
    public class GetPmHistoryQueryHandler
        : IRequestHandler<
            GetPmHistoryQuery,
            List<PmHistoryDto>>
    {
        private readonly IPmRepository _repository;

        public GetPmHistoryQueryHandler(
            IPmRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PmHistoryDto>> Handle(
            GetPmHistoryQuery request,
            CancellationToken cancellationToken)
        {
            var histories =
                await _repository.GetHistoryByDeviceIdAsync(
                    request.DeviceId, request.Tag);

            return histories
                .Select(x => new PmHistoryDto
                {
                    Id = x.Id,
                    PmScheduleId = x.PmScheduleId,
                    Status = x.Status,
                    ActionDateUtc = x.ActionDateUtc,
                    Description = x.Description,
                    //PerformedByUserId = x.PerformedByUserId,
                    CreatedAtUtc = x.CreatedAtUtc,
                    CreatedByUserId = x.CreatedByUserId,

                    DueDateSnapshot =
                        x.DueDateSnapshot,

                    TitleSnapshot =
                        x.TitleSnapshot,

                    BaseDescriptionSnapshot =
                        x.BaseDescriptionSnapshot
                })
                .ToList();
        }
    }
}