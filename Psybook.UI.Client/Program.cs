using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Psybook.Services.UI.Clients;
using Psybook.Services.UI.DataLoaders;
using Psybook.Shared.Communication;
using Psybook.Shared.Extensions;
using Psybook.UI.Client.Renderers;

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

// Data Loader Service
builder.Services.AddScoped<IBookingLoaderService, BookingDataLoaderService>();
builder.Services.AddScoped<IBookingClient, BookingClient>();

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
