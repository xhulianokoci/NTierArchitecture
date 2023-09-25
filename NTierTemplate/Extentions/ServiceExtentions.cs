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
using Shared.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Scheduler.Utility;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Entities.Exeptions;
using XSystem.Security.Cryptography;

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

    public static void ConfigureTokenLifetime(this IServiceCollection services) =>
        services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));

    public static void ConfigureBasicAuth(this IServiceCollection services)
    {
        services.AddAuthentication()
          .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", options => { });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("BasicAuthentication", new AuthorizationPolicyBuilder("BasicAuthentication").RequireAuthenticatedUser().Build());
        });
    }

    public static void ConfigureDefaultConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var emailConfig = configuration.GetSection("DefaultConfiguration").Get<DefaultConfiguration>();
        services.AddSingleton(emailConfig);
    }

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

    public static void ConfigureJwtUtils(this IServiceCollection services) =>
        services.AddSingleton<IJwtUtils, JwtUtils>();

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

    public static void AddCryptographyUtils(this IServiceCollection services) =>
        services.AddScoped<ICryptoUtils, CryptoUtils>();

    public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<JwtConfiguration>(configuration.GetSection("JwtSettings"));

    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfiguration = new JwtConfiguration();
        configuration.Bind(jwtConfiguration.Section, jwtConfiguration);

        var secretKey = jwtConfiguration.SecretKey;
        byte[] derivedKey = Derive256BitKey(secretKey);

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtConfiguration.ValidIssuer,
                ValidAudience = jwtConfiguration.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(derivedKey)
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // If the request is for our hub
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("/hubs/reservation")))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });
    }

    #region Private Methods

    private static byte[] Derive256BitKey(string secretKey)
    {
        using (var sha256 = new SHA256Managed())
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            return sha256.ComputeHash(keyBytes);
        }
    }

    #endregion
}
