using FluentValidation;
using Microsoft.Extensions.Localization;
using PhysicalPersonsDirectory.Application.Commands;
using PhysicalPersonsDirectory.Application.Validators;

public class RemoveRelatedPersonCommandValidator : AbstractValidator<RemoveRelatedPersonCommand>
{
    public RemoveRelatedPersonCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage(localizer["IdRequired"].Value);

        RuleFor(x => x.RelatedPhysicalPersonId)
            .GreaterThan(0).WithMessage(localizer["RelatedPhysicalPersonIdRequired"].Value)
            .NotEqual(x => x.Id).WithMessage(localizer["CannotRelateToSelf"].Value);
    }
}