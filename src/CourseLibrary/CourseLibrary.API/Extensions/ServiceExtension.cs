using CourseLibrary.API.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
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

        services.AddHttpCacheHeaders(
            (expirationModelOptions) =>
            {
                expirationModelOptions.MaxAge = 60;
                expirationModelOptions.CacheLocation =
                    Marvin.Cache.Headers.CacheLocation.Public;
            },
            (validationModelOptions) =>
            {
                validationModelOptions.MustRevalidate = true;
            });

        services.AddResponseCaching();
        services.AddRateLimiter(_ => _
       .AddFixedWindowLimiter(policyName: "fixed", options =>
       {
           options.PermitLimit = 4;
           options.Window = TimeSpan.FromSeconds(12);
           options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
           options.QueueLimit = 2;
       }));

        services.AddControllers(configure =>
        {

            configure.ReturnHttpNotAcceptable = true;
            configure.CacheProfiles.Add("120SecondsCacheProfile",
                new() { Duration = 120 });

            configure.Filters.Add(
       new ProducesResponseTypeAttribute(
           StatusCodes.Status400BadRequest));
            configure.Filters.Add(
                new ProducesResponseTypeAttribute(
                    StatusCodes.Status406NotAcceptable));
            configure.Filters.Add(
                new ProducesResponseTypeAttribute(
                    StatusCodes.Status500InternalServerError));
            configure.Filters.Add(new ProducesDefaultResponseTypeAttribute());
            configure.Filters.Add(
              new ConsumesAttribute("application/json", "application/json-patch+json", "application/vnd.vik.hateoas+json", "application/*+json"));
            configure.Filters.Add(
             new ProducesAttribute("application/json", "application/vnd.vik.hateoas+json", "application/xml", "application/vnd.vik.hateoas+xml"));

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

        services.Configure<MvcOptions>(config =>
        {
            var newtonsoftJsonOutputFormatter = config.OutputFormatters
                  .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

            if (newtonsoftJsonOutputFormatter != null)
            {
                newtonsoftJsonOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.vik.hateoas+json");
            }

            var xmlOutputFormatter = config.OutputFormatters
                  .OfType<XmlSerializerOutputFormatter>()?.FirstOrDefault();

            if (xmlOutputFormatter != null)
            {
                xmlOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.vik.hateoas+xml");
            }

            var jsonOutputFormatter = config.OutputFormatters
                  .OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();

            if (jsonOutputFormatter != null)
            {
                // remove text/json as it isn't the approved media type
                // for working with JSON at API level
                if (jsonOutputFormatter.SupportedMediaTypes.Contains("text/json"))
                {
                    jsonOutputFormatter.SupportedMediaTypes.Remove("text/json");
                }
            }

        });
        services.AddApiVersioning(setupAction =>
        {
            setupAction.AssumeDefaultVersionWhenUnspecified = true;
            setupAction.DefaultApiVersion = new ApiVersion(1, 0);
            setupAction.ReportApiVersions = true;
        });
        services.AddVersionedApiExplorer(setupAction =>
        {
            setupAction.GroupNameFormat = "'v'VV";
        });

        var apiVersionDescriptionProvider =services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
           

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

            foreach (var description in
            apiVersionDescriptionProvider!.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    $"LibraryOpenAPISpecification{description.GroupName}", new()
                    {
                        Title = "Library API",
                        Version = description.ApiVersion.ToString(),
                        Description = "Through this API you can access authors and their courses.",
                        Contact = new()
                        {
                            Name = "Vikash Chauhan",
                            Email="SampleAPI@github.com",
                            Url = new Uri("https://github.com/VikashChauhan51")
                        },
                        License = new()
                        {
                            Name = "MIT License",
                            Url = new Uri("https://opensource.org/licenses/MIT")
                        }
                    });
            }

            options.DocInclusionPredicate((documentName, apiDescription)
                =>
            {
                var actionApiVersionModel = apiDescription.ActionDescriptor
                   .GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

                if (actionApiVersionModel == null)
                {
                    return true;
                }

                if (actionApiVersionModel.DeclaredApiVersions.Any())
                {
                    return actionApiVersionModel.DeclaredApiVersions.Any(v =>
                    $"LibraryOpenAPISpecificationv{v}" == documentName);
                }
                return actionApiVersionModel.ImplementedApiVersions.Any(v =>
                    $"LibraryOpenAPISpecificationv{v}" == documentName);
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


        services.AddOptions();

        var section = Configuration.GetSection("Authentication");
        services.Configure<AuthenticationConfiguration>(section);
        return services;
    }
}
