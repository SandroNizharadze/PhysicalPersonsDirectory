using FluentValidation;
using PhysicalPersonsDirectory.Application.Commands;
using System.Text.RegularExpressions;

namespace PhysicalPersonsDirectory.Application.Validators;

public class UpdatePhysicalPersonCommandValidator : AbstractValidator<UpdatePhysicalPersonCommand>
{
    public UpdatePhysicalPersonCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("IdRequired");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstNameRequired")
            .Length(2, 50).WithMessage("FirstNameLength")
            .Must(BeGeorgianOrLatin).WithMessage("FirstNameGeorgianOrLatin");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastNameRequired")
            .Length(2, 50).WithMessage("LastNameLength")
            .Must(BeGeorgianOrLatin).WithMessage("LastNameGeorgianOrLatin");

        RuleFor(x => x.PersonalNumber)
            .NotEmpty().WithMessage("PersonalNumberRequired")
            .Length(11).WithMessage("PersonalNumberLength")
            .Matches(@"^\d+$").WithMessage("PersonalNumberDigits");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("DateOfBirthRequired")
            .Must(BeAtLeast18YearsOld).WithMessage("DateOfBirthMinimumAge");

        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage("CityIdRequired");

        RuleFor(x => x.PhoneNumbers)
            .NotEmpty().WithMessage("PhoneNumbersRequired");

        RuleForEach(x => x.PhoneNumbers)
            .SetValidator(new PhoneNumberDtoValidator());
    }

    private bool BeGeorgianOrLatin(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        var georgian = @"^[\u10A0-\u10FF]+$";
        var latin = @"^[a-zA-Z]+$";
        return Regex.IsMatch(name, georgian) || Regex.IsMatch(name, latin);
    }

    private bool BeAtLeast18YearsOld(DateTime dateOfBirth)
    {
        return dateOfBirth <= DateTime.UtcNow.AddYears(-18);
    }
}