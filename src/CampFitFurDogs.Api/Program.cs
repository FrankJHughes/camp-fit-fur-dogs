using CampFitFurDogs.Api.Horizontals.Hosting;
using CampFitFurDogs.Api.Horizontals.Startup;

var builder = WebApplication.CreateBuilder(args);

await Hosting.AdaptToHostingEnvironment(builder);

Startup.AddAllServices(builder);

var app = builder.Build();

Startup.UseAllServices(app);

app.Run();



