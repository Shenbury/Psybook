using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Psybook.Services.API.BookingService;
using Psybook.Services.ExternalCalendar;
using Psybook.Services.Reporting;
using Psybook.Services.Reporting.Visualization;
using Psybook.Services.UI.Clients;
using Psybook.Services.UI.DataLoaders;
using Psybook.Shared.Communication;
using Psybook.Shared.Extensions;
using Psybook.UI.Client.Renderers;
using Psybook.UI.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

// Configure BookingClient options
builder.Services.Configure<BookingClientOptions>(options =>
{
    // Configure client-specific options
    options.RequestTimeout = TimeSpan.FromSeconds(60); // Longer timeout for WebAssembly
    options.MaxItemsPerRequest = 500; // Smaller batch size for client
    options.ValidateResponses = true;
});

// HTTP Client Services - Required for ExternalCalendarService
builder.Services.AddHttpClient();

// Data Loader Service
builder.Services.AddScoped<IBookingLoaderService, BookingDataLoaderService>();
builder.Services.AddScoped<IBookingClient, BookingClient>();
builder.Services.AddScoped<IBookingService, BookingService>();

// External Calendar Integration - Register after HttpClient
builder.Services.AddScoped<IExternalCalendarService, ExternalCalendarService>();

// Reporting & Analytics Services
builder.Services.AddScoped<IDataVisualizationService, DataVisualizationService>();

// UI Services following Single Responsibility Principle
builder.Services.AddSingleton<IThemeService, ThemeService>();
builder.Services.AddScoped<INavigationService, NavigationService>();

// Add Render Context for the Client.
builder.Services.AddSingleton<IRenderContext, ClientRenderContext>();

builder.Services.ClientAndServerRegistrations();
builder.Services.AddServiceDiscovery();

builder.Services.AddHttpClient("psybook-api", https => https.BaseAddress = new Uri("https://psybook-api")).AddServiceDiscovery();

builder.Services.ConfigureHttpClientDefaults(static http =>
{
    // Turn on service discovery by default
    http.AddServiceDiscovery();
});

await builder.Build().RunAsync();
