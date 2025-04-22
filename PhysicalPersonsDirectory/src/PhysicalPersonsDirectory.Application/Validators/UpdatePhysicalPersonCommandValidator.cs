using FluentValidation;
using Microsoft.Extensions.Localization;
using PhysicalPersonsDirectory.Application.Commands;
using PhysicalPersonsDirectory.Application.DTOs;
using System.Text.RegularExpressions;

namespace PhysicalPersonsDirectory.Application.Validators;

public class UpdatePhysicalPersonCommandValidator : AbstractValidator<UpdatePhysicalPersonCommand>
{
    public UpdatePhysicalPersonCommandValidator(IStringLocalizer<SharedResources> localizer)
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage(localizer["IdRequired"].Value);

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
            .NotEmpty().WithMessage(localizer["DateOfBirthRequired"].Value)
            .Must(BeAtLeast18YearsOld).WithMessage(localizer["DateOfBirthMinimumAge"].Value);

        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage(localizer["CityIdRequired"].Value);

        RuleFor(x => x.PhoneNumbers)
            .NotEmpty().WithMessage(localizer["PhoneNumbersRequired"].Value);

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

