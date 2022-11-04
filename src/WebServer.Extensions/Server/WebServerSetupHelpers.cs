using System.Reflection;
using AV.Household.WebServer.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

namespace AV.Household.WebServer.Extensions.Server;

public static class WebServerSetupHelpers
{
    private static bool _configureSwagger = true;
    
    /// <summary>
    /// Default microservice setup
    /// </summary>
    /// <param name="builder">Webapp builder</param>
    /// <returns>Webapp ready to run</returns>
    public static WebApplication BuildAPIMicroservice(this WebApplicationBuilder builder)
    {
        ConfigureSerilog();
        ConfigureJwtParameters();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        var assemblyFullName = Assembly.GetEntryAssembly()!.GetName();
        ConfigureSwaggerGen(assemblyFullName);

        var app = builder.Build();
        LogGreetingsMessage(assemblyFullName);
        
        if(_configureSwagger) app.UseSwagger();
        ConfigureDevelopmentEnvironment(assemblyFullName);

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();

        return app;

        void ConfigureJwtParameters()
        {
            var jwtOptions = builder.Configuration.GetSection(nameof(Jwt)).Get<Jwt>();

            if (jwtOptions is null)
                throw new ApplicationException(
                    "JWT is not configured for microservice. Add configuration section to appsettings.json.");
            
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = jwtOptions.GetSymmetricSecurityKey(),
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = $"*.{jwtOptions.Issuer}",
                        ValidateLifetime = true
                    };
                });
        }

        void ConfigureSerilog()
        {
            builder.Host
                .ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders())
                .UseSerilog((webHostBuilderContext, loggerConfiguration) =>
                        loggerConfiguration.ReadFrom.Configuration(webHostBuilderContext.Configuration, "Serilog"),
                    preserveStaticLogger: false,
                    writeToProviders: false);
        }

        void ConfigureSwaggerGen(AssemblyName assemblyName)
        {
            var documentationFile = $"{AppContext.BaseDirectory}{assemblyName.Name}.xml";
            _configureSwagger = File.Exists(documentationFile);
            
            if(!_configureSwagger)
                return;
            
            builder.Services.AddSwaggerGen(c =>
            {
                c.SupportNonNullableReferenceTypes();
                c.SwaggerDoc($"v{assemblyName.Version?.Major ?? 1}", new OpenApiInfo
                {
                    Title = assemblyName.Name,
                    Version = assemblyName.Version?.ToString()
                });
                c.IncludeXmlComments(documentationFile);
            });
        }

        void LogGreetingsMessage(AssemblyName assemblyName)
        {
            app.Logger.LogInformation("Starting {AssemblyName} ver {AssemblyVersion}...", 
                assemblyName.Name, assemblyFullName.Version);
        }

        void ConfigureDevelopmentEnvironment(AssemblyName assemblyName)
        {
            if (!app.Environment.IsDevelopment()) return;
            
            app.Logger.LogInformation($"Use development exception page");
            app.UseDeveloperExceptionPage();

            app.Logger.LogInformation($"Use Swagger UI.");
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v{assemblyName.Version?.Major ?? 1}/swagger.json", assemblyName.Name);
                c.RoutePrefix = "swagger";
            });
        }
    }
}
