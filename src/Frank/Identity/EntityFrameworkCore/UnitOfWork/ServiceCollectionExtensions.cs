using Frank.Core.Application.Abstractions.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.EntityFrameworkCore.UnitOfWork;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityUnitOfWorkInfrastructure(this IServiceCollection services)
    {
        return services.AddScoped<IFrankIdentityUnitOfWork, FrankIdentityUnitOfWork>();
    }
}
