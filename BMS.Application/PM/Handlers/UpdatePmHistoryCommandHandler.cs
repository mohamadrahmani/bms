using BMS.Application.Common.Interfaces;
using BMS.Application.Models;
using BMS.Application.Pm.Commands;
using BMS.Domain.Enums;
using MediatR;

namespace BMS.Application.Pm.Handlers;

public sealed class UpdatePmHistoryCommandHandler
    : IRequestHandler<UpdatePmHistoryCommand, ApiResponse<bool>>
{
    private readonly IPmRepository _repository;

    public UpdatePmHistoryCommandHandler(IPmRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<bool>> Handle(
        UpdatePmHistoryCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Status != PmServiceStatus.Completed &&
            request.Status != PmServiceStatus.Cancelled)
        {
            return ApiResponse<bool>.FailResponse(null, "Invalid PM service status.");
        }

        if (request.ActionDateUtc == default)
        {
            return ApiResponse<bool>.FailResponse(null, "Action date is required.");
        }

        var history = await _repository.GetHistoryByIdAsync(request.Id);
        if (history is null)
        {
            return ApiResponse<bool>.FailResponse(null, "PM history not found.", 404);
        }

        history.Update(request.Status, request.ActionDateUtc, request.Description);
        await _repository.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "PM history updated successfully.");
    }
}
