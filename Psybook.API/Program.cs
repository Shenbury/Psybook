using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Psybook.Repositories.Booking;
using Psybook.ServiceDefaults;
using Psybook.Services.API.BookingService;
using Psybook.Services.ExternalCalendar;
using Psybook.Services.ExternalCalendar.GoogleCalendar;
using Psybook.Shared.Contexts;
using Psybook.Shared.Dictionary;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi()
            .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
            .AddInMemoryTokenCaches();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_myAllowSpecificOrigins",
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                      });
});

builder.AddSqlServerDbContext<BookingContext>(connectionName: "wmsp-db");
builder.Services.AddScoped<IBookingRepository, SqlBookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ExperienceDictionary>();

// External Calendar Integration Services
builder.Services.AddScoped<IExternalCalendarService, ExternalCalendarService>();
builder.Services.AddScoped<GoogleCalendarApiService>();
builder.Services.AddHttpClient(); // Required for HTTP operations in calendar services

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("_myAllowSpecificOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
