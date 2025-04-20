using FluentValidation;
using PhysicalPersonsDirectory.Application.Commands;

namespace PhysicalPersonsDirectory.Application.Validators;

public class AddRelatedPersonCommandValidator : AbstractValidator<AddRelatedPersonCommand>
{
    public AddRelatedPersonCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("IdRequired");

        RuleFor(x => x.RelatedPhysicalPersonId)
            .GreaterThan(0).WithMessage("RelatedPhysicalPersonIdRequired")
            .NotEqual(x => x.Id).WithMessage("CannotRelateToSelf");
    }
}