using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PhysicalPersonsDirectory.Application.DTOs;
using PhysicalPersonsDirectory.Domain;
using PhysicalPersonsDirectory.Infrastructure.Repositories;
using AutoMapper;
using PhysicalPersonsDirectory.Application.Validators;

namespace PhysicalPersonsDirectory.Application.Commands;

public class CreatePhysicalPersonCommand : IRequest<int>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public Gender Gender { get; set; }
    public required string PersonalNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int CityId { get; set; }
    public required List<PhoneNumberDto> PhoneNumbers { get; set; }
}

public class CreatePhysicalPersonCommandHandler : IRequestHandler<CreatePhysicalPersonCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly ILogger<CreatePhysicalPersonCommandHandler> _logger;

    public CreatePhysicalPersonCommandHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        IStringLocalizer<SharedResources> localizer,
        ILogger<CreatePhysicalPersonCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _localizer = localizer;
        _logger = logger;
    }

    public async Task<int> Handle(CreatePhysicalPersonCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating PhysicalPerson with FirstName: {FirstName}", request.FirstName);

        var person = new PhysicalPerson(
            request.FirstName,
            request.LastName,
            request.Gender,
            request.PersonalNumber,
            request.DateOfBirth,
            request.CityId
        );

        foreach (var phoneNumberDto in request.PhoneNumbers)
        {
            person.AddPhoneNumber(new PhoneNumber(phoneNumberDto.Type, phoneNumberDto.Number));
        }

        await _unitOfWork.PhysicalPersons.AddAsync(person, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created PhysicalPerson with ID: {PersonId}", person.Id);
        return person.Id;
    }
}