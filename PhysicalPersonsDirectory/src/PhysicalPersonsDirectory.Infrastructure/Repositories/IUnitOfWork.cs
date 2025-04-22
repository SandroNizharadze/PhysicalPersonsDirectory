using PhysicalPersonsDirectory.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace PhysicalPersonsDirectory.Infrastructure.Repositories;

public interface IUnitOfWork
{
    IRepository<PhysicalPerson> PhysicalPersons { get; } 
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); 
}