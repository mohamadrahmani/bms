//using BMS.Application.Points.Commands;
//using FluentValidation;

//namespace BMS.Application.Points.Validators;

//public class UpdatePointCommandValidator : AbstractValidator<UpdatePointCommand>
//{
//    public UpdatePointCommandValidator()
//    {
//        RuleFor(x => x.Id)
//            .NotEmpty();

//        RuleFor(x => x.Dto.Title)
//            .NotEmpty();

//        RuleFor(x => x.Dto.Address)
//    .NotEmpty()
//    .OverridePropertyName("Address");



//    }
//}
using BMS.Application.Points.Commands;
using FluentValidation;
using BMS.Domain.Entities.BMS;

namespace BMS.Application.Points.Validators;

public class UpdatePointCommandValidator : AbstractValidator<UpdatePointCommand>
{
    public UpdatePointCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.Address)
            .NotEmpty();

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
