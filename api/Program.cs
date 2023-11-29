using api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Startup startup = new(builder.Configuration);

startup.ConfigurationService(builder.Services);

WebApplication app = builder.Build();

startup.ConfigurationApplication(app, app.Environment);
app.Run();
