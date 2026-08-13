using Amazon.S3;
using EGL.Kinexa.Application.Interfaces;
using EGL.Kinexa.Domain.Entities;
using EGL.Kinexa.Infrastructure.Identity;
using EGL.Kinexa.Infrastructure.Services;
using EGL.Kinexa.Persistence.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using System.Text;

namespace EGL.Kinexa.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
        })
        .AddEntityFrameworkStores<KinexaDbContext>()
        .AddDefaultTokenProviders();

        var jwtSettings = configuration.GetSection("JwtSettings");
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
            };
        });

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IFileService, S3FileService>();
        services.AddScoped<IEmailService, SendGridEmailService>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var awsConfig = new Amazon.S3.AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(configuration["AWS:Region"] ?? "us-east-1")
            };
            return new Amazon.S3.AmazonS3Client(awsConfig);
        });
        var sendGridApiKey = configuration["SendGrid:ApiKey"];
        if (!string.IsNullOrEmpty(sendGridApiKey) && sendGridApiKey != "your-sendgrid-api-key")
        {
            services.AddSingleton<ISendGridClient>(new SendGridClient(sendGridApiKey));
        }
        else
        {
            services.AddSingleton<ISendGridClient>(new SendGridClient("SG.placeholder-key-for-development"));
        }
        
        services.AddMemoryCache();

        var allowedOrigins = configuration["AllowedOrigins"]?.Split(',') ?? Array.Empty<string>();
        services.AddCors(options =>
        {
            options.AddPolicy("KinexaCors", builder =>
            {
                builder.WithOrigins(allowedOrigins)
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        return services;
    }
}
