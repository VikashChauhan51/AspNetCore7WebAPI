using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using static System.Net.Mime.MediaTypeNames;

namespace CourseLibrary.API.Extensions;
public static class ServiceExtension {
    public static IServiceCollection CongigureServices(this IServiceCollection services)
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
        }).AddNewtonsoftJson().AddXmlSerializerFormatters();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
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

        return services;
    }
}
