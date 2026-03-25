using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Interfaces;
using BMS.Application.Models;
using BMS.Application.Persons.Commands;
using BMS.Domain.Entities;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;
using BMS.Domain.Exceptions;
using MediatR;


namespace BMS.Application.Persons.Handlers;

public sealed class CreatePersonCommandHandler
    : IRequestHandler<CreatePersonCommand, ApiResponse<Guid>>
{
    private readonly IPersonRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePersonCommandHandler(
        IPersonRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<Guid>> Handle(
     CreatePersonCommand request,
     CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var exists = await _repository
                .ExistsByEmailAsync(request.Email, cancellationToken);

            if (exists)
                throw new BusinessRuleException("Email already exists.");
        }

        var person = new Person(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Mobile);

        await _repository.AddAsync(person, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<Guid>.SuccessResponse(person.Id, "شخص ایجاد شد");
    }

}
