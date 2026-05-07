using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

using SharedKernel.Abstractions;

namespace SharedKernel;

public sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _provider;

    public QueryDispatcher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<TResponse> DispatchAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct)
    {
        // 1. Run validators (if any)
        var validatorType = typeof(IValidator<>).MakeGenericType(query.GetType());
        var validators = _provider.GetServices(validatorType).Cast<object>();

        var context = new ValidationContext<object>(query);

        foreach (var validator in validators)
        {
            var result = await ((IValidator)validator).ValidateAsync(context, ct);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }

        // 2. Resolve handler
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(query.GetType(), typeof(TResponse));

        var handler = (object)_provider.GetRequiredService(handlerType);

        // 3. Execute handler
        return await ((dynamic)handler).HandleAsync((dynamic)query, ct);
    }
}

