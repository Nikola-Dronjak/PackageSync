using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PackageSyncWebAPI.Infrastructure;
using PackageSyncWebAPI.Validators;
using PackageSyncWebAPI.Services;
using PackageSyncWebAPI.Models;
using PackageSyncWebAPI.Middleware;
using Serilog;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1.0.0", new OpenApiInfo
    {
        Title = "PackageSync API",
        Version = "v1.0.0",
        Description = "PackageSync API for internship at DigitalSolutions.",
        Contact = new OpenApiContact
        {
            Name = "Nikola Dronjak",
            Email = "nikola.s.dronjak@gmail.com",
            Url = new Uri("https://github.com/Nikola-Dronjak")
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("PackageSyncDb"));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddScoped<IValidator<Package>, PackageValidator>();
builder.Services.AddScoped<IPackageService, PackageService>();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1.0.0/swagger.json", "PackageSync API v1.0.0");
    });
}

app.UseMiddleware<LoggerMiddleware>();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStarted.Register(() =>
{
    var port = app.Urls.FirstOrDefault()?.Split(':').Last();
    logger.LogInformation("Server listening on port {Port}...", port);
});

lifetime.ApplicationStopping.Register(() =>
{
    logger.LogInformation("Server is shutting down...");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();