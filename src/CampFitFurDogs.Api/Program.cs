using CampFitFurDogs.Api.Horizontal.Hosting;
using CampFitFurDogs.Api.Horizontal.Startup;

var builder = WebApplication.CreateBuilder(args);

await Hosting.AdaptToHostingEnvironment(builder);

Startup.AddAllServices(builder);

var app = builder.Build();

Startup.UseAllServices(app);

app.Run();



