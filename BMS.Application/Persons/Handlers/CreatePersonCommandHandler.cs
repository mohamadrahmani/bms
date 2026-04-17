using MediatR;
using BMS.Application.Common.Interfaces;
using BMS.Domain.Entities;
using BMS.Application.Common.Exceptions;
using BMS.Application.Persons.Commands;
using BMS.Domain.Exceptions;
using BMS.Application.Models;


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
                //throw new BusinessRuleException("Email already exists.");
                throw new BusinessRuleException("این ایمیل قبلاً ثبت شده است.");

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
