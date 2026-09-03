using BMS.Application.Common.Interfaces;
using BMS.Application.Models;
using BMS.Application.Pm.Commands;
using BMS.Domain.Enums;
using MediatR;

namespace BMS.Application.Pm.Handlers
{
    public class FinalizePmCommandHandler
        : IRequestHandler<
            FinalizePmCommand,
            ApiResponse<Guid>>
    {
        private readonly IPmRepository _repository;

        public FinalizePmCommandHandler(
            IPmRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<Guid>> Handle(
            FinalizePmCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Status != PmServiceStatus.Completed &&
                request.Status != PmServiceStatus.Cancelled)
            {
                return ApiResponse<Guid>.FailResponse(null,
                    "Invalid PM service status.");
            }

            if (request.ActionDateUtc == default)
            {
                return ApiResponse<Guid>.FailResponse(null,
                    "Action date is required.");
            }

            var result = await _repository.FinalizeAsync(
                request.PmScheduleId,
                request.Status,
                request.ActionDateUtc,
                request.Description,
                //request.PerformedByUserId,
                null, // ClosedByUserId
                null  // CreatedByUserId
            );

            if (!result.HasValue)
            {
                return ApiResponse<Guid>.FailResponse(null,
                    "PM not found or already finalized.");
            }

            return ApiResponse<Guid>.SuccessResponse(
                result.Value,
                "PM finalized successfully.");
        }
    }
}
