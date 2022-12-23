using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace CourseLibrary.API.Extensions;

public static class PipelineExtension
{

    public static WebApplication CongigurePipeline(this WebApplication app, Serilog.ILogger logger, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(setupAction =>
            {
                // build a swagger endpoint for each discovered API version
                foreach (var description in
                    apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    setupAction.SwaggerEndpoint(
                        $"/swagger/LibraryOpenAPISpecification{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }

                setupAction.DefaultModelExpandDepth(2);
                setupAction.DefaultModelRendering(
                    Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                setupAction.DocExpansion(
                    Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                setupAction.EnableDeepLinking();
                setupAction.DisplayOperationId();

            });
        }
        app.ConfigureExceptionHandler(logger);
        app.UseHttpsRedirection();

        app.UseResponseCaching();
        app.UseRouting();
        app.UseHttpCacheHeaders();
        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
