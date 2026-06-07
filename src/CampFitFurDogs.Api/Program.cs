using CampFitFurDogs.Api.Horizontals.Startup;

var builder = WebApplication.CreateBuilder(args);

// ────────────────────────────────────────────────────────────────
// StartupModule Engine — Phase 1 (ConfigureServices)
// ────────────────────────────────────────────────────────────────

var configurators = StartupModuleEngine.Discover(builder.Configuration);
StartupModuleEngine.Validate(configurators);
StartupModuleEngine.RunAdd(builder, configurators);

// Build the app AFTER hosting providers + configurators have run
var app = builder.Build();

// ────────────────────────────────────────────────────────────────
// StartupModule Engine — Phase 2 (Configure)
// ────────────────────────────────────────────────────────────────

StartupModuleEngine.RunUse(app, configurators);

app.Run();

public partial class Program { }
