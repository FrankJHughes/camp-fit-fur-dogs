using System;
using System.Linq;
using System.Reflection;
using SharedKernel.DependencyInjection;

namespace SharedKernel.Tests.DependencyInjection;

public class AutoRegister_AttributeUsageTests
{
    private static readonly Assembly[] Assemblies =
    [
        typeof(AutoRegisterAttribute).Assembly
    ];

    private static readonly Type[] Known =
    [
        typeof(SharedKernel.IRepository<>),
        typeof(SharedKernel.Events.IDomainEventDispatcher),
        typeof(SharedKernel.Events.IDomainEventHandler<>),
        typeof(SharedKernel.Abstractions.ICommandDispatcher),
        typeof(SharedKernel.Abstractions.ICommandHandler<>),
        typeof(SharedKernel.Abstractions.ICommandHandler<,>),
        typeof(SharedKernel.Abstractions.ICurrentUserService),
        typeof(SharedKernel.Abstractions.IQueryDispatcher),
        typeof(SharedKernel.Abstractions.IQueryHandler<,>),
        typeof(SharedKernel.Abstractions.IUnitOfWork)
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
