using MediatR;
using PhysicalPersonsDirectory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace PhysicalPersonsDirectory.Application.Commands;

public class RemoveRelatedPersonCommand : IRequest
{
    public int Id { get; set; }
    public int RelatedPhysicalPersonId { get; set; }
}

public class RemoveRelatedPersonCommandHandler : IRequestHandler<RemoveRelatedPersonCommand>
{
    private readonly ApplicationDbContext _context;

    public RemoveRelatedPersonCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RemoveRelatedPersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.PhysicalPersons
            .Include(p => p.RelatedPersons)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (person == null)
        {
            throw new ArgumentException($"Person with ID {request.Id} not found.");
        }

        var relationship = person.RelatedPersons
            .FirstOrDefault(r => r.RelatedPhysicalPersonId == request.RelatedPhysicalPersonId);

        if (relationship == null)
        {
            throw new ArgumentException($"Relationship between person {request.Id} and related person {request.RelatedPhysicalPersonId} does not exist.");
        }

        person.RelatedPersons.Remove(relationship);

        await _context.SaveChangesAsync(cancellationToken);
    }
}