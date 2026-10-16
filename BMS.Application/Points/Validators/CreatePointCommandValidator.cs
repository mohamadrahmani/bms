using BMS.Application.Points.Commands;
using FluentValidation;
using BMS.Domain.Entities.BMS;

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
                //.NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.DeviceId)
                .NotEmpty();

            RuleFor(x => x.DataType)
                .IsInEnum();

            RuleFor(x => x.Kind)
                .IsInEnum();

            RuleFor(x => x.MinRawValue)
                .NotNull()
                .When(x => x.DataType != PointDataType.Boolean);

            RuleFor(x => x.MaxRawValue)
                .NotNull()
                .When(x => x.DataType != PointDataType.Boolean);

            RuleFor(x => x.MinDisplayValue)
                .NotNull()
                .When(x => x.DataType != PointDataType.Boolean);

            RuleFor(x => x.MaxDisplayValue)
                .NotNull()
                .When(x => x.DataType != PointDataType.Boolean);

            RuleFor(x => x.MaxRawValue)
                .Must((command, maxRawValue) => !maxRawValue.HasValue ||
                    !command.MinRawValue.HasValue || command.MinRawValue.Value != maxRawValue.Value)
                .When(x => x.DataType != PointDataType.Boolean)
                .WithMessage("Raw minimum and maximum values must differ.");
        }
    }
}
