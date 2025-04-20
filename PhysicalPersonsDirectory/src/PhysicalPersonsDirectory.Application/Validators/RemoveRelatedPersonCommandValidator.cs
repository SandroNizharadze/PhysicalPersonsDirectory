using FluentValidation;
using PhysicalPersonsDirectory.Application.Commands;

namespace PhysicalPersonsDirectory.Application.Validators;

public class RemoveRelatedPersonCommandValidator : AbstractValidator<RemoveRelatedPersonCommand>
{
    public RemoveRelatedPersonCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("IdRequired");

        RuleFor(x => x.RelatedPhysicalPersonId)
            .GreaterThan(0).WithMessage("RelatedPhysicalPersonIdRequired")
            .NotEqual(x => x.Id).WithMessage("CannotRelateToSelf");
    }
}