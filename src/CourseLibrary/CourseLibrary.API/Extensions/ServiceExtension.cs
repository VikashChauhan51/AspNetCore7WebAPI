using CourseLibrary.API.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using static System.Net.Mime.MediaTypeNames;

namespace CourseLibrary.API.Extensions;
public static class ServiceExtension
{
    public static IServiceCollection CongigureServices(this IServiceCollection services, IConfiguration Configuration)
    {

        services.AddRateLimiter(_ => _
       .AddFixedWindowLimiter(policyName: "fixed", options =>
       {
           options.PermitLimit = 4;
           options.Window = TimeSpan.FromSeconds(12);
           options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
           options.QueueLimit = 2;
       }));

        services.AddControllers(options =>
        {
            options.ReturnHttpNotAcceptable = true;
        }).ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
                new BadRequestObjectResult(context.ModelState)
                {
                    ContentTypes =
                    {
                    Application.Json,
                    Application.Xml
                    }
                };
        }).AddNewtonsoftJson(setupAction =>
        {
            setupAction.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
        }).AddXmlSerializerFormatters();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            options.AddSecurityDefinition("CourseLibraryApiBearerAuth", new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                Description = "Input a valid token to access this API"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
             {
                new OpenApiSecurityScheme
               {
                  Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "CourseLibraryApiBearerAuth" }
               }, new List<string>() }
             });
        });

        services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Configuration["Authentication:Issuer"],
            ValidAudience = Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(Configuration["Authentication:SecretForKey"]!))
        };
    }
    );

        services.AddAuthorization(options =>
        {
            options.AddPolicy("MustBeAuthenticated", policy =>
            {
                policy.RequireAuthenticatedUser();
                 
            });
        });

        services.AddApiVersioning(setupAction =>
        {
            setupAction.AssumeDefaultVersionWhenUnspecified = true;
            setupAction.DefaultApiVersion = new ApiVersion(1, 0);
            setupAction.ReportApiVersions = true;
        });

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddResponseCaching();

        services.AddHttpCacheHeaders(
            (expirationModelOptions) =>
            {
                expirationModelOptions.MaxAge = 60;
                expirationModelOptions.CacheLocation =
                    Marvin.Cache.Headers.CacheLocation.Private;
            },
            (validationModelOptions) =>
            {
                validationModelOptions.MustRevalidate = true;
            });
        services.AddOptions();

        var section = Configuration.GetSection("Authentication");
        services.Configure<AuthenticationConfiguration>(section);
        return services;
    }
}
