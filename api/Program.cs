using api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

ILogger<Startup> logger = builder
    .Services
    .BuildServiceProvider()
    .GetRequiredService<ILogger<Startup>>();

Startup startup = new(builder.Configuration, logger);

startup.ConfigureServices(builder.Services);

WebApplication app = builder.Build();

startup.Configure(app, app.Environment);

app.Run();
