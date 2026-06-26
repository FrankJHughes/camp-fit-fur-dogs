using System.Reflection;
using Frank.Abstractions.Command;
using Frank.Abstractions.Environment;
using Frank.Abstractions.Events;
using Frank.Abstractions.ExceptionHandling;
using Frank.Abstractions.Identity;
using Frank.Abstractions.Query;
using Frank.Abstractions.Time;
using Frank.Abstractions.UnitOfWork;
using Frank.AutoRegistration;

namespace Frank.Tests.DependencyInjection;

public class AutoRegister_AttributeUsageTests
{
    private static readonly Assembly[] Assemblies =
    [
        typeof(AutoRegisterAttribute).Assembly
    ];

    private static readonly Type[] Known =
    [
        typeof(ICommandDispatcher),
        typeof(ICommandHandler<>),
        typeof(ICommandHandler<,>),
        typeof(ICurrentUser),
        typeof(IQueryDispatcher),
        typeof(IQueryHandler<,>),
        typeof(IUnitOfWork),
        typeof(IEnvironment),
        typeof(IDomainEventDispatcher),
        typeof(IDomainEventHandler<>),
        typeof(IExceptionHandler),
        typeof(IClock)
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
