using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhysicalPersonsDirectory.Infrastructure.Repositories;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query();
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}