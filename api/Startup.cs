using api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<AppDBContext>(
                options =>
                    options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")),
                ServiceLifetime.Scoped
            );

            services.AddCors(options =>
            {
                string? frontendURL = Configuration.GetValue<string>("UrlFrontend");
                if (string.IsNullOrEmpty(frontendURL))
                {
                    _logger.LogError("The frontendURL configuration is null or empty.");
                    throw new InvalidOperationException("Frontend URL is not configured.");
                }
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(frontendURL).AllowAnyMethod().AllowAnyHeader();
                });
            });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // Set the schemes to both HTTP and HTTPS
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri(
                                    "/auth-server/connect/authorize",
                                    UriKind.Relative
                                ),
                                Scopes = new Dictionary<string, string>
                                {
                                    { "readAccess", "Access read operations" },
                                    { "writeAccess", "Access write operations" }
                                }
                            }
                        }
                    }
                );
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (env is null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseSwagger();
                    app.UseSwaggerUI(
                        c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1")
                    );
                }

                app.UseHttpsRedirection();
                // It's important to call UseRouting before UseAuthorization and UseEndpoints
                app.UseRouting();

                // Assuming you have authentication, it should come before UseAuthorization
                app.UseAuthentication(); // Add this line if you have authentication
                app.UseAuthorization();

                app.UseCors();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while configuring the HTTP request pipeline."
                );
                throw;
            }
        }
    }
}
