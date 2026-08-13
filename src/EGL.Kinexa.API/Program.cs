using EGL.Kinexa.Application;
using EGL.Kinexa.Application.Interfaces;
using EGL.Kinexa.Application.Settings;
using EGL.Kinexa.Infrastructure;
using EGL.Kinexa.Infrastructure.Services;
using EGL.Kinexa.Infrastructure.Middleware;
using EGL.Kinexa.Persistence;
using EGL.Kinexa.Persistence.Seed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// AWS S3
builder.Services.Configure<AwsSettings>(builder.Configuration.GetSection("AWS"));
builder.Services.AddSingleton<IFileService, S3FileService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    await SeedData.SeedAsync(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("KinexaCors");

app.UseAuthentication();
app.UseMiddleware<SingleSessionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
