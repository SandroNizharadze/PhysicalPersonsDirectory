using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhysicalPersonsDirectory.Domain.Interfaces;
using PhysicalPersonsDirectory.Infrastructure.Persistence;
using PhysicalPersonsDirectory.Infrastructure.Repositories;

namespace PhysicalPersonsDirectory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, ApplicationDbContext>();
        services.AddScoped<IPhysicalPersonRepository, PhysicalPersonRepository>();

        return services;
    }
}