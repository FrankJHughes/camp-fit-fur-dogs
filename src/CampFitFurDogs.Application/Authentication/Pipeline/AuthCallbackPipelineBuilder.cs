using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application.Authentication.Pipeline;

public sealed class AuthCallbackPipelineBuilder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<IAuthCallbackStep> _steps = [];

    public AuthCallbackPipelineBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public AuthCallbackPipelineBuilder Add<TStep>()
        where TStep : class, IAuthCallbackStep
    {
        var step = _serviceProvider.GetRequiredService<TStep>();
        _steps.Add(step);
        return this;
    }

    public AuthCallbackPipeline Build()
        => new(_steps);
}
