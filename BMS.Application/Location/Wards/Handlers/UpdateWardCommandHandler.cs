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
            throw new Exception("بخش پیدا نشد.");

        // 1️⃣ چک کنیم Floor جدید وجود دارد
        var floorExists = await _floorRepository.ExistsAsync(request.FloorId);

        if (!floorExists)
            throw new Exception("طبقه پیدا نشد.");
        // Track Changes
        TrackChange(request, "FloorId", ward.FloorId.ToString(), request.FloorId.ToString());
        TrackChange(request, "Name", ward.Name, request.Name);
        TrackChange(request, "Type", ward.Type, request.Type);
        TrackChange(request, "Description", ward.Description, request.Description);

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
    private static void TrackChange(UpdateWardCommand request, string field, string? oldValue, string? newValue)
    {
        if (oldValue != newValue)
        {
            request.Changes.Add((field, oldValue, newValue));
        }
    }
}
