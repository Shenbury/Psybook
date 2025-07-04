using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Psybook.Services.UI.DataLoaders;
using Psybook.Shared.Communication;
using Psybook.Shared.Extensions;
using Psybook.UI.Client.Renderers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

// Data Loader Service
builder.Services.AddScoped<IBookingLoaderService, BookingDataLoaderService>();

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
