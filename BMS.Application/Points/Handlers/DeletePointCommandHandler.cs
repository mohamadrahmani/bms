using BMS.Application.Common.Interfaces;
using BMS.Application.Models;
using BMS.Application.Points.Commands;
using BMS.Domain.Entities.BMS;
using MediatR;

namespace BMS.Application.Points.Handlers;

public class DeletePointCommandHandler : IRequestHandler<DeletePointCommand, ApiResponse<bool>>
{
    private readonly IPointRepository _repository;

    public DeletePointCommandHandler(IPointRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<bool>> Handle(DeletePointCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);

        return ApiResponse<bool>.SuccessResponse(true, "رجیستری حذف شد"); ;
    }
}
