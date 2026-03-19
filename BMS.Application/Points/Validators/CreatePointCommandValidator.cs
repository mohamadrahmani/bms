using BMS.Application.Points.Commands;
using FluentValidation;

namespace BMS.Application.Points.Validators
{
    public class CreatePointCommandValidator : AbstractValidator<CreatePointCommand>
    {
        public CreatePointCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Address)
                .NotNull();

            RuleFor(x => x.Tag)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.DeviceId)
                .NotEmpty();

            RuleFor(x => x.DataType)
                .IsInEnum();

            RuleFor(x => x.Kind)
                .IsInEnum();
        }
    }
}
