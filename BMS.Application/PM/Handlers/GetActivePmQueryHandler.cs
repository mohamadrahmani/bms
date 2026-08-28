using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Pm;
using BMS.Application.Models;
using BMS.Application.Pm.DTOs;
using BMS.Application.Pm.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BMS.Application.Pm.QueryHandlers
{
    public class GetActivePmQueryHandler
        : IRequestHandler<
            GetActivePmQuery,
            List<PmDto>>
    {
        private readonly IPmRepository _pmRepository;

        public GetActivePmQueryHandler(
            IPmRepository pmRepository)
        {
            _pmRepository = pmRepository;
        }

        public async Task<List<PmDto>?> Handle(
            GetActivePmQuery request,
            CancellationToken cancellationToken)
        {
            var res = await _pmRepository.PmSchedules
                .AsNoTracking()
                .Where(
                    x =>
                        x.DeviceId == request.DeviceId
                        && (string.IsNullOrEmpty(request.Tag) || x.Tag == request.Tag)
                        && x.IsActive)
                .Select(pm => new PmDto
                {
                    Id = pm.Id,
                    DeviceId = pm.DeviceId,
                    Title = pm.Title,
                    Description = pm.Description,
                    DueDate = pm.DueDate,
                    WarningDays = pm.WarningDays,
                    IsActive = pm.IsActive,
                    CreatedAtUtc = pm.CreatedAtUtc,
                    UpdatedAtUtc = pm.UpdatedAtUtc,
                    ClosedAtUtc = pm.ClosedAtUtc,
                    Tag = pm.Tag,

                    Indicator =
                    PmIndicatorCalculator.Calculate(
                        pm.DueDate,
                        pm.WarningDays)
                }).ToListAsync(cancellationToken);

            return res;
        }
    }
}