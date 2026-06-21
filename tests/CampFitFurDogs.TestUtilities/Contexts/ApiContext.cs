using Frank.Testing.Contexts;
using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.TestUtilities.Contexts;

public sealed record ApiContext
    : MutatedWebApplicationContext<ApiContext>
{
    public ApiContext()
    {
        // Without this, CORS Startup will throw.
        _ = ConfigOverrides.Append(cfg =>
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Frontend:BaseUrl"] = "http://localhost:5173"
            })
        );
    }
}
