//using BMS.Application.CommandDefinitions.Commands;
//using FluentValidation;

//namespace BMS.Application.CommandDefinitions.Validators;

//public class UpdateCommandDefinitionCommandValidator : AbstractValidator<UpdateCommandDefinitionCommand>
//{
//    public UpdateCommandDefinitionCommandValidator()
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
using BMS.Application.CommandDefinitions.Commands;
using FluentValidation;

namespace BMS.Application.CommandDefinitions.Validators;

public class UpdateCommandDefinitionCommandValidator : AbstractValidator<UpdateCommandDefinitionCommand>
{
    public UpdateCommandDefinitionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        //RuleFor(x => x.Title)
        //    .NotEmpty();

        //RuleFor(x => x.Address)
        //    .NotEmpty();
    }
}
