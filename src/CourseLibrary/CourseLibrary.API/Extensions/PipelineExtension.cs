using Microsoft.AspNetCore.Builder;

namespace CourseLibrary.API.Extensions;

public static class PipelineExtension
{

    public static WebApplication CongigurePipeline(this WebApplication app, Serilog.ILogger logger)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
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
