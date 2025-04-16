using MediatR;
using PhysicalPersonsDirectory.Application.DTOs;
using PhysicalPersonsDirectory.Domain;
using PhysicalPersonsDirectory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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
    private readonly ApplicationDbContext _context;

    public CreatePhysicalPersonCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreatePhysicalPersonCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Convert DateOfBirth to UTC if it's not already
            var dateOfBirth = request.DateOfBirth.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(request.DateOfBirth, DateTimeKind.Utc)
                : request.DateOfBirth.ToUniversalTime();

            var person = new PhysicalPerson(
                request.FirstName,
                request.LastName,
                request.Gender,
                request.PersonalNumber,
                dateOfBirth,
                request.CityId);

            Console.WriteLine($"Before Add - Person ID: {person.Id}");

            foreach (var phone in request.PhoneNumbers)
            {
                person.AddPhoneNumber(new PhoneNumber(phone.Type, phone.Number));
            }

            await _context.PhysicalPersons.AddAsync(person);
            _context.Entry(person).State = EntityState.Added;

            Console.WriteLine($"After Add - Person ID: {person.Id}");
            Console.WriteLine($"Entity State: {_context.Entry(person).State}");

            var changesSaved = await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"Changes Saved: {changesSaved}");

            Console.WriteLine($"After SaveChangesAsync - Person ID: {person.Id}");

            await _context.Entry(person).ReloadAsync(cancellationToken);
            Console.WriteLine($"After Reload - Person ID: {person.Id}");

            if (person.Id == 0)
            {
                throw new Exception("Failed to generate ID for the new PhysicalPerson after reload.");
            }

            await transaction.CommitAsync(cancellationToken);

            return person.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new Exception($"Failed to create PhysicalPerson: {ex.Message}", ex);
        }
    }
}