using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Interfaces;
using BMS.Application.Models;
using BMS.Application.Persons.Commands;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Exceptions;
using MediatR;

namespace BMS.Application.Persons.Handlers;


public sealed class UpdatePersonCommandHandler
    : IRequestHandler<UpdatePersonCommand, ApiResponse<bool>>
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

    public async Task<ApiResponse<bool>> Handle(
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

        return ApiResponse<bool>.SuccessResponse(true, "اطلاعات بروزرسانی شد");
    }
}
