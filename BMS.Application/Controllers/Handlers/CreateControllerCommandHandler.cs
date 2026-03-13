using MediatR;
using BMS.Application.Controllers.Commands;
using BMS.Application.Common.Interfaces;
using BMS.Application.Common.Exceptions;
using BMS.Domain.Entities.BMS;

namespace BMS.Application.Controllers.Handlers;

public sealed class CreateControllerCommandHandler
    : IRequestHandler<CreateControllerCommand, Guid>
{
    private readonly IControllerRepository _controllerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateControllerCommandHandler(
        IControllerRepository controllerRepository,
        IUnitOfWork unitOfWork)
    {
        _controllerRepository = controllerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateControllerCommand request,
        CancellationToken cancellationToken)
    {
        // 1️⃣ Prevent duplicate Controller Code
        var codeExists = await _controllerRepository
            .ExistsByCodeAsync(request.Code, cancellationToken);

        if (codeExists)
            throw new BusinessRuleException(
                $"Controller with code '{request.Code}' already exists.");

        // 2️⃣ Normalize Code
        var normalizedCode = request.Code
            .Trim()
            .ToUpperInvariant();

        // 3️⃣ Create Controller
        var controller = new Controller(
            normalizedCode,
            request.Name,
            request.Protocol,
            request.IpAddress,
            request.Port,
            request.UnitId,
            request.TimeoutMs,
            request.RetryCount,
            request.ScanIntervalMs,
            request.Description
        );

        // 4️⃣ Persist
        await _controllerRepository.AddAsync(controller, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return controller.Id;
    }
}
