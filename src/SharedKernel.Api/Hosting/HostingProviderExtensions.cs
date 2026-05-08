using Microsoft.AspNetCore.Builder;

namespace SharedKernel.Api.Hosting;

/// <summary>
/// Wires <see cref="IHostingProvider"/> implementations into the ASP.NET Core
/// startup pipeline.  Consuming projects call
/// <c>builder.UseHostingProviders(…)</c> in <c>Program.cs</c>, passing
/// concrete provider instances in priority order.
/// </summary>
/// <remarks>
/// Providers run <em>before</em> the DI container is built, so they cannot
/// be resolved from the service provider.  Pass concrete instances in
/// priority order instead.  First-active-wins: the first provider whose
/// <see cref="IHostingProvider.IsActive"/> returns <c>true</c> applies its
/// overrides and the rest are skipped.
/// </remarks>
public static class HostingProviderExtensions
{
    /// <summary>
    /// Evaluates each <see cref="IHostingProvider"/> in registration order.
    /// The first whose <see cref="IHostingProvider.IsActive"/> returns
    /// <c>true</c> applies its overrides; the rest are skipped.
    /// At most one provider runs per application start.
    /// </summary>
    public static async Task UseHostingProviders(
        this WebApplicationBuilder builder,
        params IHostingProvider[] providers)
    {
        foreach (var provider in providers)
        {
            if (!provider.IsActive())
            {
                Log($"{provider.ProviderName} — not detected, skipping.");
                continue;
            }

            Log($"{provider.ProviderName} — detected, applying overrides…");

            try
            {
                await provider.ConfigureAsync(builder);
                Log($"{provider.ProviderName} — overrides applied successfully.");
            }
            catch (Exception ex)
            {
                Log($"{provider.ProviderName} — override failed: {ex.Message}");
            }

            return; // first-active-wins
        }

        Log("No hosting provider matched the current environment.");
    }

    private static void Log(string message)
        => Console.WriteLine($"[Hosting] {message}");
}
