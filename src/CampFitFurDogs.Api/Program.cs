using CampFitFurDogs.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// ────────────────────────────────────────────────────────────────
// Configurator Engine — Phase 1 (ConfigureServices)
// ────────────────────────────────────────────────────────────────

var configurators = ConfiguratorEngine.Discover(builder.Configuration);
ConfiguratorEngine.Validate(configurators);
ConfiguratorEngine.RunConfigureServices(builder, configurators);

// Build the app AFTER hosting providers + configurators have run
var app = builder.Build();

// ────────────────────────────────────────────────────────────────
// Configurator Engine — Phase 2 (Configure)
// ────────────────────────────────────────────────────────────────

ConfiguratorEngine.RunConfigure(app, configurators);

app.Run();

public partial class Program { }
