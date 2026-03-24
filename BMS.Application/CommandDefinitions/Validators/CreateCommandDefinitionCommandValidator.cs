using BMS.Application.CommandDefinitions.Commands;
using FluentValidation;

namespace BMS.Application.CommandDefinitions.Validators
{
    public class CreateCommandDefinitionCommandValidator : AbstractValidator<CreateCommandDefinitionCommand>
    {
        public CreateCommandDefinitionCommandValidator()
        {
            //RuleFor(x => x.Title)
            //    .NotEmpty()
            //    .MaximumLength(100);

            //RuleFor(x => x.Address)
            //    .NotNull();

            //RuleFor(x => x.Tag)
            //    .NotEmpty()
            //    .MaximumLength(50);

            //RuleFor(x => x.DeviceId)
            //    .NotEmpty();

            //RuleFor(x => x.DataType)
            //    .IsInEnum();

            //RuleFor(x => x.Kind)
            //    .IsInEnum();
        }
    }
}
