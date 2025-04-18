using FluentValidation;
using PhysicalPersonsDirectory.Application.Commands;

namespace PhysicalPersonsDirectory.Application.Validators;

public class DeletePhysicalPersonCommandValidator : AbstractValidator<DeletePhysicalPersonCommand>
{
    public DeletePhysicalPersonCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("IdRequired");
    }
}