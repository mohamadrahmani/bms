using BMS.Application.Common.Interfaces;
using BMS.Application.Models;
using BMS.Application.Pm.Commands;
using MediatR;

namespace BMS.Application.Pm.Handlers
{
    public class UpdatePmCommandHandler
        : IRequestHandler<UpdatePmCommand, ApiResponse<bool>>
    {
        private readonly IPmRepository _repository;

        public UpdatePmCommandHandler(
            IPmRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<bool>> Handle(
            UpdatePmCommand request,
            CancellationToken cancellationToken)
        {
            var pm = await _repository.GetByIdAsync(request.Id);

            if (pm == null)
            {
                return ApiResponse<bool>.FailResponse(null,
                    "PM not found.");
            }

            if (!pm.IsActive)
            {
                return ApiResponse<bool>.FailResponse(null,
                    "Closed PM cannot be updated.");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return ApiResponse<bool>.FailResponse(null,
                    "PM title is required.");
            }

            if (request.WarningDays < 0)
            {
                return ApiResponse<bool>.FailResponse(null,
                    "Warning days cannot be negative.");
            }

            pm.Update(
                request.Title,
                request.Description,
                request.DueDate,
                request.WarningDays);

            await _repository.UpdateAsync(pm);
            await _repository.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(
                true,
                "PM updated successfully.");
        }
    }
}