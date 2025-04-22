using MediatR;
using PhysicalPersonsDirectory.Domain;
using Microsoft.EntityFrameworkCore;
using PhysicalPersonsDirectory.Infrastructure;

namespace PhysicalPersonsDirectory.Application.Commands;

public class AddRelatedPersonCommand : IRequest
{
    public int Id { get; set; }
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
        var person = await _context.PhysicalPersons
            .Include(p => p.RelatedPersons)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (person == null)
        {
            throw new ArgumentException($"Person with ID {request.Id} not found.");
        }

        var relatedPersonExists = await _context.PhysicalPersons
            .AnyAsync(p => p.Id == request.RelatedPhysicalPersonId, cancellationToken);

        if (!relatedPersonExists)
        {
            throw new ArgumentException($"Related person with ID {request.RelatedPhysicalPersonId} not found.");
        }

        if (person.RelatedPersons.Any(r => r.RelatedPhysicalPersonId == request.RelatedPhysicalPersonId))
        {
            throw new ArgumentException($"Relationship between person {request.Id} and related person {request.RelatedPhysicalPersonId} already exists.");
        }

        person.RelatedPersons.Add(new RelatedPerson(request.RelatedPhysicalPersonId, request.RelationType));

        await _context.SaveChangesAsync(cancellationToken);
    }
}