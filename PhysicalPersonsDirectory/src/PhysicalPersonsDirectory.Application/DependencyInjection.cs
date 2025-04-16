using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PhysicalPersonsDirectory.Application.Commands;
using PhysicalPersonsDirectory.Application.Mapping;
using System.Reflection;

namespace PhysicalPersonsDirectory.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddAutoMapper(typeof(PhysicalPersonProfile).Assembly);
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<CreatePhysicalPersonCommandHandler>();
        return services;
    }
}