using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PackageSyncWebAPI.Infrastructure;
using PackageSyncWebAPI.Validators;
using PackageSyncWebAPI.Services;
using PackageSync.Domain;
using PackageSyncWebAPI.Middleware;
using Serilog;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5020")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

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

    var libraryXmlPath = Path.Combine(AppContext.BaseDirectory, "PackageSync.Domain.xml");
    options.IncludeXmlComments(libraryXmlPath);
});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("PackageSyncDb"));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IValidator<Package>, PackageValidator>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

UserSeeder.SeedUsers(app).Wait();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1.0.0/swagger.json", "PackageSync API v1.0.0");
    });
}

app.UseCors();

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();