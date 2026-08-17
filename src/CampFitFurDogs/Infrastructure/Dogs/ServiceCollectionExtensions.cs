using CampFitFurDogs.Application.Abstractions.Dogs;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDogById;
using CampFitFurDogs.Application.Abstractions.Dogs.GetDog;
using CampFitFurDogs.Application.Abstractions.Dogs.ListDogsByOwner;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Application.Abstractions.Dogs.RemoveDog;
using CampFitFurDogs.Application.Abstractions.Dogs.RegisterDog;

namespace CampFitFurDogs.Infrastructure.Dogs;

/// <summary>
/// Provides extension methods for registering infrastructure‑layer services
/// related to the Dogs vertical slice.
/// <para>
/// This class wires up all EF Core‑backed readers and writers used by the
/// application layer’s dog‑related workflows.
/// </para>
/// <para>
/// The registrations follow the scoped lifetime convention, ensuring each
/// operation receives its own <c>AppDbContext</c> instance.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all infrastructure‑layer services for the Dogs vertical slice.
    /// <para>
    /// This includes:
    /// <list type="bullet">
    /// <item><description><see cref="IEditDogWriter"/> → <see cref="EditDogWriter"/></description></item>
    /// <item><description><see cref="IRegisterDogWriter"/> → <see cref="RegisterDogWriter"/></description></item>
    /// <item><description><see cref="IRemoveDogWriter"/> → <see cref="RemoveDogWriter"/></description></item>
    /// <item><description><see cref="IGetDogByIdReader"/> → <see cref="GetDogByIdReader"/></description></item>
    /// <item><description><see cref="IGetDogReader"/> → <see cref="GetDogReader"/></description></item>
    /// <item><description><see cref="IListDogsByOwnerReader"/> → <see cref="ListDogsByOwnerReader"/></description></item>
    /// </list>
    /// </para>
    /// <para>
    /// These services collectively support all dog‑related application workflows,
    /// including registration, editing, deletion, and querying.
    /// </para>
    /// </summary>
    /// <param name="services">The DI service collection.</param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with all Dogs infrastructure
    /// services registered.
    /// </returns>
    public static IServiceCollection AddInfrastructureDogs(this IServiceCollection services)
    {
        return services
            .AddScoped<IEditDogWriter, EditDogWriter>()
            .AddScoped<IRegisterDogWriter, RegisterDogWriter>()
            .AddScoped<IRemoveDogWriter, RemoveDogWriter>()
            .AddScoped<IGetDogByIdReader, GetDogByIdReader>()
            .AddScoped<IGetDogReader, GetDogReader>()
            .AddScoped<IListDogsByOwnerReader, ListDogsByOwnerReader>();
    }
}
