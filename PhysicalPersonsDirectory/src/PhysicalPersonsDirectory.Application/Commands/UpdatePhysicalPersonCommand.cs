using MediatR;
using PhysicalPersonsDirectory.Application.DTOs;
using PhysicalPersonsDirectory.Domain;
using PhysicalPersonsDirectory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace PhysicalPersonsDirectory.Application.Commands;

public class UpdatePhysicalPersonCommand : IRequest
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public Gender Gender { get; set; }
    public required string PersonalNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int CityId { get; set; }
    public required string CityName { get; set; }
    public required List<PhoneNumberDto> PhoneNumbers { get; set; }
    public string? ImagePath { get; set; }
    public required List<RelatedPersonDto> RelatedPersons { get; set; }
}

public class UpdatePhysicalPersonCommandHandler : IRequestHandler<UpdatePhysicalPersonCommand>
{
    private readonly ApplicationDbContext _context;

    public UpdatePhysicalPersonCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdatePhysicalPersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.PhysicalPersons
            .Include(p => p.PhoneNumbers)
            .Include(p => p.RelatedPersons)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (person == null)
        {
            throw new Exception($"Person with ID {request.Id} not found.");
        }

        var dateOfBirth = request.DateOfBirth.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.DateOfBirth, DateTimeKind.Utc)
            : request.DateOfBirth.ToUniversalTime();

        person.Update(
            request.FirstName,
            request.LastName,
            request.Gender,
            request.PersonalNumber,
            dateOfBirth,
            request.CityId);

        person.ImagePath = request.ImagePath;

        person.PhoneNumbers.Clear();
        foreach (var phoneDto in request.PhoneNumbers)
        {
            person.AddPhoneNumber(new PhoneNumber(phoneDto.Type, phoneDto.Number));
        }

        person.RelatedPersons.Clear();
        foreach (var relatedDto in request.RelatedPersons)
        {
            person.RelatedPersons.Add(new RelatedPerson(relatedDto.RelatedPhysicalPersonId, relatedDto.RelationType));
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}