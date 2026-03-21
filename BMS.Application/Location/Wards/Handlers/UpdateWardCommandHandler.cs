using BMS.Application.Common.Interfaces;
using MediatR;

namespace BMS.Application.Location.Wards.Commands;

public class UpdateWardCommandHandler : IRequestHandler<UpdateWardCommand>
{
    private readonly IWardRepository _repository;
    private readonly IFloorRepository _floorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWardCommandHandler(
        IWardRepository repository,
        IFloorRepository floorRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _floorRepository = floorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateWardCommand request, CancellationToken cancellationToken)
    {
        var ward = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (ward == null)
            throw new Exception("Ward not found");

        // 1️⃣ چک کنیم Floor جدید وجود دارد
        var floorExists = await _floorRepository.ExistsAsync(request.FloorId);

        if (!floorExists)
            throw new Exception("Floor not found");

        // 2️⃣ بروزرسانی با FloorId جدید
        ward.Update(
            request.FloorId,
            request.Name,
            request.Type,
            request.Description
        );

        _repository.Update(ward);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
