using MediatR;
using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Interfaces;
using BMS.Domain.Exceptions;
using BMS.Application.Persons.Commands;
using BMS.Application.Common.Audit;
using BMS.Application.Models;

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
                  // $"Person with id '{request.Id}' not found.");
                  $"شخصی با شناسه '{request.Id}' یافت نشد.");


        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var exists = await _personRepository
                .ExistsByEmailAsync(request.Email.Trim(), cancellationToken);

            if (exists && !string.Equals(
                    person.Email,
                    request.Email.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                //throw new BusinessRuleException("Email already exists.");
                throw new BusinessRuleException("این ایمیل قبلاً ثبت شده است.");

            }
        }
        // ✅ ذخیره مقادیر قبلی برای Audit
        var oldFirstName = person.FirstName;
        var oldLastName = person.LastName;
        var oldEmail = person.Email;
        var oldMobile = person.Mobile;

        person.Update(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Mobile);

        // ✅ تشخیص تغییرات
        var changes = EntityChangeDetector.GetPersonChanges(
            oldFirstName,
            oldLastName,
            oldEmail,
            oldMobile,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Mobile);
        // ✅ ارسال تغییرات به AuditBehavior
        request.Changes = changes;


        await _unitOfWork.SaveChangesAsync(cancellationToken);
       
        return ApiResponse<bool>.SuccessResponse(true, "اطلاعات بروزرسانی شد");
    }
}
