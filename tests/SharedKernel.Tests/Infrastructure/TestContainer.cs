
namespace SharedKernel.Tests.TestInfrastructure;

public sealed class TestContainer
{
    private readonly IServiceCollection _services = new ServiceCollection();

    public static TestContainer Create() => new();

    public TestContainer WithCommandHandler<TCommand, TResponse, THandler>()
        where TCommand : ICommand<TResponse>
        where THandler : class, ICommandHandler<TCommand, TResponse>
    {
        _services.AddTransient<ICommandHandler<TCommand, TResponse>, THandler>();
        return this;
    }

    public TestContainer WithCommandHandler<TCommand, THandler>()
        where TCommand : ICommand
        where THandler : class, ICommandHandler<TCommand>
    {
        _services.AddTransient<ICommandHandler<TCommand>, THandler>();
        return this;
    }

    public TestContainer WithQueryHandler<TQuery, TResponse, THandler>()
        where TQuery : IQuery<TResponse>
        where THandler : class, IQueryHandler<TQuery, TResponse>
    {
        _services.AddTransient<IQueryHandler<TQuery, TResponse>, THandler>();
        return this;
    }

    public TestContainer WithValidator<T, TValidator>()
        where TValidator : class, IValidator<T>
    {
        _services.AddTransient<IValidator<T>, TValidator>();
        return this;
    }

    public TestContainer WithInstance<TService>(TService instance)
        where TService : class
    {
        _services.AddSingleton(instance);
        return this;
    }

    public TestContainer WithDispatcher<TDispatcher, TInterface>()
        where TDispatcher : class, TInterface
        where TInterface : class
    {
        _services.AddTransient<TInterface, TDispatcher>();
        return this;
    }

    public IServiceProvider Build() => _services.BuildServiceProvider();
}

