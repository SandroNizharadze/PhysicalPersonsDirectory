using MediatR;
using PhysicalPersonsDirectory.Domain;
using PhysicalPersonsDirectory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace PhysicalPersonsDirectory.Application.Commands;

public class AddRelatedPersonCommand : IRequest
{
    public int Id { get; set; } // PhysicalPerson Id
    public int RelatedPhysicalPersonId { get; set; }
    public RelationType RelationType { get; set; }
}

public class AddRelatedPersonCommandHandler : IRequestHandler<AddRelatedPersonCommand>
{
    private readonly ApplicationDbContext _context;

    public AddRelatedPersonCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AddRelatedPersonCommand request, CancellationToken cancellationToken)
    {
        // Check if the physical person exists
        var person = await _context.PhysicalPersons
            .Include(p => p.RelatedPersons)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (person == null)
        {
            throw new ArgumentException($"Person with ID {request.Id} not found.");
        }

        // Check if the related person exists
        var relatedPersonExists = await _context.PhysicalPersons
            .AnyAsync(p => p.Id == request.RelatedPhysicalPersonId, cancellationToken);

        if (!relatedPersonExists)
        {
            throw new ArgumentException($"Related person with ID {request.RelatedPhysicalPersonId} not found.");
        }

        // Check if the relationship already exists
        if (person.RelatedPersons.Any(r => r.RelatedPhysicalPersonId == request.RelatedPhysicalPersonId))
        {
            throw new ArgumentException($"Relationship between person {request.Id} and related person {request.RelatedPhysicalPersonId} already exists.");
        }

        // Add the new relationship
        person.RelatedPersons.Add(new RelatedPerson(request.RelatedPhysicalPersonId, request.RelationType));

        await _context.SaveChangesAsync(cancellationToken);
    }
}