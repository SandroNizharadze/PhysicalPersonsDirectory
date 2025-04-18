using MediatR;
using PhysicalPersonsDirectory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace PhysicalPersonsDirectory.Application.Commands;

public class DeletePhysicalPersonCommand : IRequest
{
    public int Id { get; set; }
}

public class DeletePhysicalPersonCommandHandler : IRequestHandler<DeletePhysicalPersonCommand>
{
    private readonly ApplicationDbContext _context;

    public DeletePhysicalPersonCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeletePhysicalPersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.PhysicalPersons
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (person == null)
        {
            throw new Exception($"Person with ID {request.Id} not found.");
        }

        _context.PhysicalPersons.Remove(person);
        await _context.SaveChangesAsync(cancellationToken);
    }
}