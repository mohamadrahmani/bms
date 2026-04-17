using MediatR;
using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Interfaces;
using BMS.Application.Persons.Commands;

namespace BMS.Application.Persons.Handlers;

public sealed class DeletePersonCommandHandler
    : IRequestHandler<DeletePersonCommand>
{
    private readonly IPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePersonCommandHandler(
        IPersonRepository personRepository,
        IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        DeletePersonCommand request,
        CancellationToken cancellationToken)
    {
        var person = await _personRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (person is null)
            throw new NotFoundException(
                $"شخصی با شناسه '{request.Id}' یافت نشد.");

        person.Deactivate(); // Soft delete

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
