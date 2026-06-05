using Microsoft.AspNetCore.Builder;

namespace Frank.Api.Hosting;

public static class HostingProviderExtensions
{
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

                // 🔥 HARDENING: Fail fast when an active provider cannot configure itself
                throw new InvalidOperationException(
                    $"Hosting provider '{provider.ProviderName}' failed to configure the application. " +
                    $"Startup aborted to protect production integrity.", ex);
            }

            return; // first-active-wins
        }

        Log("No hosting provider matched the current environment.");
    }

    private static void Log(string message)
        => Console.WriteLine($"[Hosting] {message}");
}
