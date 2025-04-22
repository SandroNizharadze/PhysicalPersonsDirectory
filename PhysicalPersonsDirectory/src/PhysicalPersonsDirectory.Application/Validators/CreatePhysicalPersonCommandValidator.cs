using FluentValidation;
using Microsoft.Extensions.Localization;
using PhysicalPersonsDirectory.Application.Commands;
using PhysicalPersonsDirectory.Application.DTOs;
using System.Text.RegularExpressions;

namespace PhysicalPersonsDirectory.Application.Validators;

public class CreatePhysicalPersonCommandValidator : AbstractValidator<CreatePhysicalPersonCommand>
{
    public CreatePhysicalPersonCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(localizer["FirstNameRequired"].Value)
            .Length(2, 50).WithMessage(localizer["FirstNameLength"].Value)
            .Must(BeGeorgianOrLatin).WithMessage(localizer["FirstNameGeorgianOrLatin"].Value);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(localizer["LastNameRequired"].Value)
            .Length(2, 50).WithMessage(localizer["LastNameLength"].Value)
            .Must(BeGeorgianOrLatin).WithMessage(localizer["LastNameGeorgianOrLatin"].Value);

        RuleFor(x => x.PersonalNumber)
            .NotEmpty().WithMessage(localizer["PersonalNumberRequired"].Value)
            .Length(11).WithMessage(localizer["PersonalNumberLength"].Value)
            .Matches(@"^\d+$").WithMessage(localizer["PersonalNumberDigits"].Value);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage(localizer["DateOfBirthRequired"])
            .Must(BeAtLeast18YearsOld).WithMessage(localizer["DateOfBirthMinimumAge"]);

        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage(localizer["CityIdRequired"]);

        RuleFor(x => x.PhoneNumbers)
            .NotEmpty().WithMessage(localizer["PhoneNumbersRequired"]);

        RuleForEach(x => x.PhoneNumbers)
            .SetValidator(new PhoneNumberDtoValidator(localizer));
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

public class PhoneNumberDtoValidator : AbstractValidator<PhoneNumberDto>
{
    public PhoneNumberDtoValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage(localizer["PhoneNumberRequired"].Value)
            .Matches(@"^\+995\d{9}$").WithMessage(localizer["PhoneNumberFormat"].Value);

        RuleFor(x => x.Type)
            .NotNull().WithMessage(localizer["PhoneNumberTypeRequired"].Value);
    }
}