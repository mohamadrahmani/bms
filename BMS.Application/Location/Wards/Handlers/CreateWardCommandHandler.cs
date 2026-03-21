using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities.Location;
using MediatR;

namespace BMS.Application.Location.Wards.Commands;

public class CreateWardCommandHandler : IRequestHandler<CreateWardCommand, Guid>
{
    private readonly IWardRepository _repository;
    private readonly IFloorRepository _floorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWardCommandHandler(
        IWardRepository repository,
        IFloorRepository floorRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _floorRepository = floorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateWardCommand request, CancellationToken cancellationToken)
    {
        var floorExists = await _floorRepository.ExistsAsync(request.FloorId);

        if (!floorExists)
            throw new Exception("Floor not found");

        var ward = new Ward(
            request.FloorId,
            request.Name,
            request.Type,
            request.Description);

        await _repository.AddAsync(ward, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ward.Id;
    }
}
