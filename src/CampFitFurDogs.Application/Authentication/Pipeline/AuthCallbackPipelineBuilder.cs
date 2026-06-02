using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application.Authentication.Pipeline;

public sealed class AuthCallbackPipelineBuilder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<IAuthCallbackStep> _steps = [];

    // ------------------------------------------------------------
    // Canonical workflow order (REAL order from DI registration)
    // ------------------------------------------------------------
    private static readonly AuthCallbackStepCategory[] CategoryOrder =
    {
        AuthCallbackStepCategory.Precondition,
        AuthCallbackStepCategory.ExchangeCode,
        AuthCallbackStepCategory.FetchUser,
        AuthCallbackStepCategory.ValidateUser,
        AuthCallbackStepCategory.ResolveIdentity,
        AuthCallbackStepCategory.IssueCookie,
        AuthCallbackStepCategory.CreateSession,
        AuthCallbackStepCategory.AuditLogin,
        AuthCallbackStepCategory.BuildRedirect,
        AuthCallbackStepCategory.Finalize
    };

    // ------------------------------------------------------------
    // Required categories (must appear exactly once)
    // ------------------------------------------------------------
    private static readonly AuthCallbackStepCategory[] RequiredExactlyOnce =
    {
        AuthCallbackStepCategory.ExchangeCode,
        AuthCallbackStepCategory.FetchUser,
        AuthCallbackStepCategory.ValidateUser,
        AuthCallbackStepCategory.ResolveIdentity,
        AuthCallbackStepCategory.IssueCookie,
        AuthCallbackStepCategory.CreateSession,
        AuthCallbackStepCategory.AuditLogin,
        AuthCallbackStepCategory.BuildRedirect
    };

    // ------------------------------------------------------------
    // Optional categories (0 or 1)
    // ------------------------------------------------------------
    private static readonly AuthCallbackStepCategory[] OptionalAtMostOne =
    {
        AuthCallbackStepCategory.Precondition
    };

    // ------------------------------------------------------------
    // Multi-step categories (0 or many)
    // ------------------------------------------------------------
    private static readonly AuthCallbackStepCategory[] MultiStepAllowed =
    {
        AuthCallbackStepCategory.Precondition
    };

    public static IReadOnlyList<AuthCallbackStepCategory> DefaultCategoryOrder => CategoryOrder;

    public AuthCallbackPipelineBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    // ------------------------------------------------------------
    // Automatic DI discovery
    // ------------------------------------------------------------
    public AuthCallbackPipelineBuilder DiscoverSteps()
    {
        var discovered = _serviceProvider.GetServices<IAuthCallbackStep>().ToList();

        if (discovered.Count == 0)
            throw new InvalidOperationException("No IAuthCallbackStep implementations registered in DI.");

        _steps.AddRange(discovered);
        return this;
    }

    // ------------------------------------------------------------
    // Manual Add (still supported)
    // ------------------------------------------------------------
    public AuthCallbackPipelineBuilder Add<TStep>()
        where TStep : class, IAuthCallbackStep
    {
        var step = _serviceProvider.GetRequiredService<TStep>();
        _steps.Add(step);
        return this;
    }

    private int FindIndex<TStep>() where TStep : class, IAuthCallbackStep =>
        _steps.FindIndex(s => s.GetType() == typeof(TStep));

    // ------------------------------------------------------------
    // Build
    // ------------------------------------------------------------
    public AuthCallbackPipeline Build(
       bool discoveryEnabled = true,
       bool validationEnabled = true)
    {
        if (discoveryEnabled)
            DiscoverSteps();

        SortStepsByCategory();

        if (validationEnabled)
        {
            ValidateCategoryOrdering();
            ValidateRequiredCategories();
            ValidateCategoryCardinality();
        }

        return new AuthCallbackPipeline(_steps);
    }

    // ------------------------------------------------------------
    // Sorting
    // ------------------------------------------------------------
    private void SortStepsByCategory()
    {
        _steps.Sort((a, b) =>
        {
            var aIndex = Array.IndexOf(CategoryOrder, a.Metadata.Category);
            var bIndex = Array.IndexOf(CategoryOrder, b.Metadata.Category);
            return aIndex.CompareTo(bIndex);
        });
    }

    // ------------------------------------------------------------
    // 1. Enforce canonical ordering
    // ------------------------------------------------------------
    private void ValidateCategoryOrdering()
    {
        AuthCallbackStepCategory? last = null;

        foreach (var step in _steps)
        {
            var category = step.Metadata.Category;

            if (!CategoryOrder.Contains(category))
                throw new InvalidOperationException(
                    $"Unknown step category '{category}' in step '{step.Metadata.Id}'.");

            if (last is not null)
            {
                var lastIndex = Array.IndexOf(CategoryOrder, last.Value);
                var currentIndex = Array.IndexOf(CategoryOrder, category);

                if (currentIndex < lastIndex)
                {
                    throw new InvalidOperationException(
                        $"Step '{step.Metadata.Id}' (category {category}) appears out of order. " +
                        $"Expected category >= {last}.");
                }
            }

            last = category;
        }
    }

    // ------------------------------------------------------------
    // 2. Ensure required categories exist exactly once
    // ------------------------------------------------------------
    private void ValidateRequiredCategories()
    {
        foreach (var required in RequiredExactlyOnce)
        {
            var count = _steps.Count(s => s.Metadata.Category == required);

            if (count == 0)
                throw new InvalidOperationException(
                    $"Required pipeline category '{required}' must appear exactly once, but is missing.");

            if (count > 1)
                throw new InvalidOperationException(
                    $"Pipeline category '{required}' must appear exactly once, but appears {count} times.");
        }
    }

    // ------------------------------------------------------------
    // 3. Enforce cardinality rules
    // ------------------------------------------------------------
    private void ValidateCategoryCardinality()
    {
        foreach (var category in OptionalAtMostOne)
        {
            var count = _steps.Count(s => s.Metadata.Category == category);

            if (count > 1)
                throw new InvalidOperationException(
                    $"Pipeline category '{category}' may appear at most once, but appears {count} times.");
        }

        foreach (var step in _steps)
        {
            var category = step.Metadata.Category;

            if (!RequiredExactlyOnce.Contains(category) &&
                !OptionalAtMostOne.Contains(category) &&
                !MultiStepAllowed.Contains(category))
            {
                throw new InvalidOperationException(
                    $"Category '{category}' is not configured with cardinality rules.");
            }
        }
    }
}
