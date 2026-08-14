using BMS.Application.PreventiveMaintenance.Commands;
using FluentValidation;

namespace BMS.Application.PreventiveMaintenance.Validators;

public sealed class CreatePmScheduleCommandValidator : AbstractValidator<CreatePmScheduleCommand>
{
    public CreatePmScheduleCommandValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.DueDateUtc).NotEmpty();
        RuleFor(x => x.WarningDays).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdatePmScheduleCommandValidator : AbstractValidator<UpdatePmScheduleCommand>
{
    public UpdatePmScheduleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.DueDateUtc).NotEmpty();
        RuleFor(x => x.WarningDays).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RowVersion).NotEmpty();
    }
}

public sealed class FinalizePmScheduleCommandValidator : AbstractValidator<FinalizePmScheduleCommand>
{
    public FinalizePmScheduleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.ActionDateUtc).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.RowVersion).NotEmpty();
    }
}

public sealed class AddPmScheduleAttachmentCommandValidator
    : AbstractValidator<AddPmScheduleAttachmentCommand>
{
    public AddPmScheduleAttachmentCommandValidator()
    {
        RuleFor(x => x.PmScheduleId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(150);
        RuleFor(x => x.FileContent).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}

public sealed class AddPmHistoryAttachmentCommandValidator
    : AbstractValidator<AddPmHistoryAttachmentCommand>
{
    public AddPmHistoryAttachmentCommandValidator()
    {
        RuleFor(x => x.PmServiceHistoryId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(150);
        RuleFor(x => x.FileContent).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
