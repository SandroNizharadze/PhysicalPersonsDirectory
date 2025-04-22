using Microsoft.EntityFrameworkCore;
using PhysicalPersonsDirectory.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace PhysicalPersonsDirectory.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IRepository<PhysicalPerson> _physicalPersons;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _physicalPersons = new Repository<PhysicalPerson>(context);
    }

    public IRepository<PhysicalPerson> PhysicalPersons => _physicalPersons;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}