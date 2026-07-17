using System.Reflection;
using Frank.Core.Application.Abstractions.Cqrs.Commands;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Core.Application.Abstractions.DomainEvents;
using Frank.Core.Application.Abstractions.Exceptions;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Frank.Core.Application.Registration;

namespace Frank.Core.Application.Tests.Registration;

public class AutoRegister_AttributeUsageTests
{
    private static readonly Assembly[] Assemblies =
    [
        typeof(RegistrationAttribute).Assembly
    ];

    private static readonly Type[] Known =
    [
        typeof(ICommandHandler<>),
        typeof(ICommandHandler<,>),
        typeof(IDomainEventHandler<>),
        typeof(IExceptionHandler),
        typeof(IQueryHandler<,>),
        typeof(IEndpoint)
    ];

    [Fact]
    public void No_Interface_Should_Have_AutoRegister_Unless_Intentional()
    {
        var offenders =
            (from asm in Assemblies
             from type in asm.DefinedTypes
             let attr = type.GetCustomAttribute<RegistrationAttribute>()
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
