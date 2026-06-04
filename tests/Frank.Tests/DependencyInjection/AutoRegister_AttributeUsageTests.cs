using System.Reflection;
using Frank.DependencyInjection;

namespace Frank.Tests.DependencyInjection;

public class AutoRegister_AttributeUsageTests
{
    private static readonly Assembly[] Assemblies =
    [
        typeof(AutoRegisterAttribute).Assembly
    ];

    private static readonly Type[] Known =
    [
        typeof(Frank.IRepository<>),
        typeof(Frank.Events.IDomainEventDispatcher),
        typeof(Frank.Events.IDomainEventHandler<>),
        typeof(Frank.Abstractions.ICommandDispatcher),
        typeof(Frank.Abstractions.ICommandHandler<>),
        typeof(Frank.Abstractions.ICommandHandler<,>),
        typeof(Frank.Abstractions.ICurrentUserService),
        typeof(Frank.Abstractions.IQueryDispatcher),
        typeof(Frank.Abstractions.IQueryHandler<,>),
        typeof(Frank.Abstractions.IUnitOfWork)
    ];

    [Fact]
    public void No_Interface_Should_Have_AutoRegister_Unless_Intentional()
    {
        var offenders =
            (from asm in Assemblies
             from type in asm.DefinedTypes
             let attr = type.GetCustomAttribute<AutoRegisterAttribute>()
             where type.IsInterface
                   && attr is not null
                   && !Known.Contains(type.AsType())
             select type.FullName)
            .ToList();

        Assert.False(offenders.Any(),
            "Unexpected interfaces marked with [AutoRegister]:\n" +
            string.Join("\n", offenders));
    }
}
