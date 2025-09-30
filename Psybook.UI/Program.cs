using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using MudBlazor.Services;
using Psybook.ServiceDefaults;
using Psybook.Services.API.BookingService;
using Psybook.Services.ExternalCalendar;
using Psybook.Services.Monitoring;
using Psybook.Services.Reporting;
using Psybook.Services.Reporting.Visualization;
using Psybook.Services.UI.Clients;
using Psybook.Services.UI.DataLoaders;
using Psybook.Shared.Communication;
using Psybook.Shared.Extensions;
using Psybook.UI.Client.Services;
using Psybook.UI.Components;
using Psybook.UI.Renderers;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire Defaults
builder.AddServiceDefaults();

// Add Authentication & Authorization (Basic setup for development)
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

// Add MudBlazor services
builder.Services.AddMudServices();

builder.Services.ClientAndServerRegistrations();

// RenderContext communicates to components in which RenderMode the component is running.
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IRenderContext, ServerRenderContext>();

// Configure BookingClient options
builder.Services.Configure<BookingClientOptions>(
    builder.Configuration.GetSection("BookingClient"));

// Configure HTTP clients (without complex auth for now)
builder.Services.AddHttpClient("psybook-api", client => 
{
    client.BaseAddress = new Uri("https://psybook-api");
})
.AddServiceDiscovery();

// External Calendar Integration - Register after HttpClient
builder.Services.AddScoped<IExternalCalendarService, ExternalCalendarService>();

// Reporting & Analytics Services
builder.Services.AddScoped<IDataVisualizationService, DataVisualizationService>();

// Monitoring Services - Use Scoped for UI to get fresh metrics per request
builder.Services.AddScoped<ISystemMetricsService, SystemMetricsService>();

// Data Loader Service
builder.Services.AddScoped<IBookingLoaderService, BookingDataLoaderService>();
builder.Services.AddScoped<IBookingClient, BookingClient>();

builder.Services.AddScoped<IExperienceLoaderService, ExperienceDataLoaderService>();
builder.Services.AddScoped<IExperienceClient, ExperienceClient>();

// Reporting Client
builder.Services.AddScoped<IReportingClient, ReportingClient>();

// UI Services following Single Responsibility Principle
builder.Services.AddSingleton<IThemeService, ThemeService>();
builder.Services.AddScoped<INavigationService, NavigationService>();

builder.Services.ConfigureHttpClientDefaults(static http =>
{
    // Turn on service discovery by default
    http.AddServiceDiscovery();
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Psybook.UI.Client._Imports).Assembly);

app.MapDefaultEndpoints();

app.Run();
