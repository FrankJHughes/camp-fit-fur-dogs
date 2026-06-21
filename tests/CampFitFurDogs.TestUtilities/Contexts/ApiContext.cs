using Frank.Testing.Contexts;
using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.TestUtilities.Contexts;

public sealed record ApiContext
    : MutatedWebApplicationContext<ApiContext>
{
    public override IReadOnlyList<Action<IConfigurationBuilder>> ConfigOverrides { get; init; }
        =
        [
            cfg => cfg.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    // Without this, CORS Startup will throw.
                    ["Frontend:BaseUrl"] = "http://localhost:5173"
                })
        ];
}
