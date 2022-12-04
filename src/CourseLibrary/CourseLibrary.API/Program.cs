
using Serilog;

Log.Logger = SerilogConfigurationHelper.Congigure();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.

builder.Services
    .CongigureServices()
    .CongigureRepositories();

var app = builder.Build();
app.UseRateLimiter();
// Configure the HTTP request pipeline.
app.CongigurePipeline().Run();  