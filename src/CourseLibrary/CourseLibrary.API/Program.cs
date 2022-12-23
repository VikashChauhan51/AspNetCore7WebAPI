
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Serilog;

Log.Logger = SerilogConfigurationHelper.Congigure();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.

builder.Services
    .CongigureServices(builder.Configuration)
    .CongigureRepositories(builder.Configuration);

IApiVersionDescriptionProvider? apiVersionDescriptionProvider = builder.Services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();
var app = builder.Build();
app.UseRateLimiter();
// Configure the HTTP request pipeline.
app.CongigurePipeline(Log.Logger, apiVersionDescriptionProvider).Run();  
