using System.Reflection;
using FluentAssertions;
using Frank.Infrastructure.EntityFrameworkCore.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CampFitFurDogs.Architecture.Tests;

public class AppDbContextAutoDiscoveryGuardrailTests
{
    [Fact]
    public void AppDbContext_Should_Have_No_DbSet_Properties()
    {
        var dbSetProperties = typeof(FrankIdentityDbContext)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType
                     && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .Select(p => p.Name)
            .ToList();

        dbSetProperties.Should().BeEmpty(
            "AppDbContext must not expose DbSet<T> properties — " +
            "use Set<T>() for auto-discovery (US-107)");
    }
}
