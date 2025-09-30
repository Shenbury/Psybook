using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Psybook.API.Controllers;
using Psybook.Objects.DbModels;
using Psybook.Repositories.Booking;
using Psybook.Repositories.Experience;
using Psybook.ServiceDefaults;
using Psybook.Services.API.BookingService;
using Psybook.Services.API.ExperienceService;
using Psybook.Services.Background;
using Psybook.Services.ExternalCalendar;
using Psybook.Services.Middleware;
using Psybook.Services.Monitoring;
using Psybook.Services.Reporting;
using Psybook.Services.Reporting.Visualization;
using Psybook.Shared.Contexts;
using Psybook.Shared.Extensions;
using Psybook.Shared.Dictionary;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults
builder.AddServiceDefaults();

// Add SQL Server database context - using the correct database name from AppHost
builder.AddSqlServerDbContext<BookingContext>("wmsp-db");

// Add Authentication - Configure JWT for API access
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        options.TokenValidationParameters.ValidAudiences = new[]
        {
            options.Audience,
            builder.Configuration["AzureAd:ClientId"],
            "api://psybook-api"
        };
    }, options =>
    {
        builder.Configuration.Bind("AzureAd", options);
    });

// Add Authorization
builder.Services.AddAuthorization();
// Note: Fallback policy removed for development - controllers use [AllowAnonymous]

// Add Controllers and API Explorer
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "WMSP Booking API", 
        Version = "v1",
        Description = "West Midlands Safari Park VIP Experience Booking System API - Employee Access Only"
    });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add CORS - Update origins for different environments
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7154", 
                "http://localhost:5154",
                "https://localhost:7031",  // UI server origin
                "http://localhost:5031"    // UI server origin
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add HTTP Client Factory
builder.Services.AddHttpClient();

// Repository Services
builder.Services.AddScoped<IBookingRepository, SqlBookingRepository>();
builder.Services.AddScoped<IExperienceRepository, SqlExperienceRepository>();

// Shared Dictionary Services
builder.Services.AddScoped<ExperienceDictionary>();

// Business Logic Services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();

// Reporting Services
builder.Services.AddScoped<IReportingService, ReportingService>();
builder.Services.AddScoped<IDataVisualizationService, DataVisualizationService>();

// Monitoring Services
builder.Services.AddSingleton<ISystemMetricsService, SystemMetricsService>();

// External Integration Services
builder.Services.AddScoped<IExternalCalendarService, ExternalCalendarService>();

// Background Services
builder.Services.AddHostedService<ScheduledReportingService>();

// Shared Services
builder.Services.ClientAndServerRegistrations();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WMSP Booking API v1");
        c.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
        c.OAuthUsePkce();
    });
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Add metrics tracking middleware early in the pipeline
app.UseMetricsTracking();

app.UseCors("AllowBlazorClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();
