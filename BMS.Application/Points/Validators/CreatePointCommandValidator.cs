using BMS.Application.Points.Commands;
using FluentValidation;

namespace BMS.Application.Points.Validators;

public class CreatePointCommandValidator : AbstractValidator<CreatePointCommand>
{
    public CreatePointCommandValidator()
    {
        RuleFor(x => x.Dto.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Dto.Address)
            .NotNull();
    }
}
