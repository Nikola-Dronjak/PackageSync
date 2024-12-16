using Microsoft.EntityFrameworkCore;
using PackageSyncWebAPI.Infrastructure;
using PackageSyncWebAPI.Services;
using Serilog;
using PackageSyncWebAPI.Middleware;
using FluentValidation;
using PackageSyncWebAPI.Models;
using PackageSyncWebAPI.Validators;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseSwaggerUI();
}

app.UseMiddleware<LoggerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();