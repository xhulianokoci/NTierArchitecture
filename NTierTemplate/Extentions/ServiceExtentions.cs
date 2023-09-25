using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NTierTemplate.Filters;
using Repository;
using Repository.Contracts;
using Service;
using Service.Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using LoggerService;

namespace NTierTemplate.Extentions;

public static class ServiceExtentions
{
    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerManager, LoggerManager>();

    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();

    public static void ConfigureServiceManager(this IServiceCollection services) =>
    services.AddScoped<IServiceManager, ServiceManager>();

    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>(opts =>
            opts.UseSqlServer(configuration.GetConnectionString("sqlConnection"), b => b.MigrationsAssembly("NTierTemplate"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking), ServiceLifetime.Transient);

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        var builder = services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
        {
            o.Password.RequireDigit = true;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequiredLength = 8;
            o.Password.RequireNonAlphanumeric = false;
            //o.User.RequireUniqueEmail = true;
            //o.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
            //o.Lockout.AllowedForNewUsers = true;
            //o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            //o.Lockout.MaxFailedAccessAttempts = 3;
        }).AddEntityFrameworkStores<RepositoryContext>().AddDefaultTokenProviders();
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "NTierTemplate API",
                Version = "v1",
                Description = "NTierTemplate API by XK"
            });

            //var xmlFile = $"{typeof(Presantation.AssemblyReference).Assembly.GetName().Name}.xml";
            //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //s.IncludeXmlComments(xmlPath);

            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Place to add JWT with Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            s.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer",
                        },
                        new List<string>()
                    }
            });

            s.OperationFilter<AcceptLanguageFilter>();
        });
    }
}
