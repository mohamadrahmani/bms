using MediatR;
using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Interfaces;
using BMS.Domain.Exceptions;
using BMS.Application.Persons.Commands;

namespace BMS.Application.Persons.Handlers;


public sealed class UpdatePersonCommandHandler
    : IRequestHandler<UpdatePersonCommand, Unit>
{
    private readonly IPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePersonCommandHandler(
        IPersonRepository personRepository,
        IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        UpdatePersonCommand request,
        CancellationToken cancellationToken)
    {
        var person = await _personRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (person is null)
            throw new NotFoundException(
                $"Person with id '{request.Id}' not found.");

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var exists = await _personRepository
                .ExistsByEmailAsync(request.Email.Trim(), cancellationToken);

            if (exists && !string.Equals(
                    person.Email,
                    request.Email.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessRuleException("Email already exists.");
            }
        }

        person.Update(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Mobile);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;   // 👈 خیلی مهم
    }
}
