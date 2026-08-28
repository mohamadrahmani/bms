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
            ApiResponse<bool>>
    {
        private readonly IPmRepository _repository;

        public FinalizePmCommandHandler(
            IPmRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<bool>> Handle(
            FinalizePmCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Status != PmServiceStatus.Completed &&
                request.Status != PmServiceStatus.Cancelled)
            {
                return ApiResponse<bool>.FailResponse(null,
                    "Invalid PM service status.");
            }

            if (request.ActionDateUtc == default)
            {
                return ApiResponse<bool>.FailResponse(null,
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

            if (!result)
            {
                return ApiResponse<bool>.FailResponse(null,
                    "PM not found or already finalized.");
            }

            return ApiResponse<bool>.SuccessResponse(
                true,
                "PM finalized successfully.");
        }
    }
}