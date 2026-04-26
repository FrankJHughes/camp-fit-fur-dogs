
namespace SharedKernel.Tests.TestInfrastructure;

public abstract class DispatcherTestBase
{
    private readonly TestContainer _container = TestContainer.Create();

    protected IServiceProvider Provider { get; private set; } = default!;

    protected void BuildContainer() => Provider = _container.Build();

    protected void WithCommandHandler<TCommand, THandler>()
        where TCommand : ICommand
        where THandler : class, ICommandHandler<TCommand>
        => _container.WithCommandHandler<TCommand, THandler>();

    protected void WithCommandHandler<TCommand, TResponse, THandler>()
        where TCommand : ICommand<TResponse>
        where THandler : class, ICommandHandler<TCommand, TResponse>
        => _container.WithCommandHandler<TCommand, TResponse, THandler>();

    protected void WithQueryHandler<TQuery, TResponse, THandler>()
        where TQuery : IQuery<TResponse>
        where THandler : class, IQueryHandler<TQuery, TResponse>
        => _container.WithQueryHandler<TQuery, TResponse, THandler>();

    protected void WithValidator<T, TValidator>()
        where TValidator : class, IValidator<T>
        => _container.WithValidator<T, TValidator>();

    protected void WithInstance<TService>(TService instance)
        where TService : class
        => _container.WithInstance(instance);

    protected void WithDispatcher<TDispatcher, TInterface>()
        where TDispatcher : class, TInterface
        where TInterface : class
        => _container.WithDispatcher<TDispatcher, TInterface>();
}

