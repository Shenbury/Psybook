using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Psybook.Services.UI.DataLoaders;
using Psybook.Shared.Communication;
using Psybook.Shared.Extensions;
using Psybook.UI.Client.Renderers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

// Data Loader Service
builder.Services.AddScoped(typeof(IDataLoaderService<>), typeof(CustomDataLoaderService<>));

// Add Render Context for the Client.
builder.Services.AddSingleton<IRenderContext, ClientRenderContext>();

builder.Services.ClientAndServerRegistrations();

await builder.Build().RunAsync();
