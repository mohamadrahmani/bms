using BMS.Application.Common.Interfaces;
using BMS.Application.Location.Wards.Commands;
using MediatR;

public class DeleteWardCommandHandler : IRequestHandler<DeleteWardCommand>
{
    private readonly IWardRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWardCommandHandler(
        IWardRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        DeleteWardCommand request,
        CancellationToken cancellationToken)
    {
        var ward = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (ward == null)
            throw new Exception("بخش پیدا نشد.");

        _repository.Remove(ward);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
