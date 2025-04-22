using FluentValidation;
using Microsoft.Extensions.Localization;
using PhysicalPersonsDirectory.Application.Commands;

namespace PhysicalPersonsDirectory.Application.Validators;

public class AddRelatedPersonCommandValidator : AbstractValidator<AddRelatedPersonCommand>
{
    public AddRelatedPersonCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage(localizer["IdRequired"].Value);

        RuleFor(x => x.RelatedPhysicalPersonId)
            .GreaterThan(0).WithMessage(localizer["RelatedPhysicalPersonIdRequired"].Value)
            .NotEqual(x => x.Id).WithMessage(localizer["CannotRelateToSelf"].Value);
    }
}

public class SharedResources
{
}