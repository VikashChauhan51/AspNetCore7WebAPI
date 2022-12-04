using Serilog;

namespace CourseLibrary.API.Helpers;

public static class SerilogConfigurationHelper
{
    public static Serilog.Core.Logger Congigure()
    {
        return new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/CourseLibrary.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
    }
}
