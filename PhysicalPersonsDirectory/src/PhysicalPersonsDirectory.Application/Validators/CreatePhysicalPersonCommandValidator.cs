using FluentValidation;
using PhysicalPersonsDirectory.Application.Commands;

namespace PhysicalPersonsDirectory.Application.Validators;

public class CreatePhysicalPersonCommandValidator : AbstractValidator<CreatePhysicalPersonCommand>
{
    public CreatePhysicalPersonCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(2, 50)
            .Must(BeGeorgianOrLatin)
            .WithMessage("First name must be 2-50 characters and contain only Georgian or Latin letters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(2, 50)
            .Must(BeGeorgianOrLatin)
            .WithMessage("Last name must be 2-50 characters and contain only Georgian or Latin letters.");

        RuleFor(x => x.PersonalNumber)
            .NotEmpty()
            .Length(11)
            .Matches(@"^\d{11}$")
            .WithMessage("Personal number must be 11 digits.");

        RuleFor(x => x.DateOfBirth)
            .Must(dob => dob <= DateTime.Now.AddYears(-18))
            .WithMessage("Person must be at least 18 years old.");

        RuleForEach(x => x.PhoneNumbers)
            .ChildRules(phone =>
            {
                phone.RuleFor(p => p.Number)
                    .Length(4, 50)
                    .WithMessage("Phone number must be 4-50 characters.");
            });
    }

    private bool BeGeorgianOrLatin(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        bool isGeorgian = text.All(c => c >= 0x10D0 && c <= 0x10FF);
        bool isLatin = text.All(c => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'));
        return isGeorgian || isLatin;
    }
}