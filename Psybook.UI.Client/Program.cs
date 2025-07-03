using Application.Common.Interfaces;
using Application.Features.CustomTable;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Psybook.Shared.Communication;
using Psybook.UI.Client.Contexts;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

// Data Loader Service
builder.Services.AddScoped(typeof(IDataLoaderService<>), typeof(CustomDataLoaderService<>));

// Add Render Context for the Client.
builder.Services.AddSingleton<IRenderContext, ClientRenderContext>();

await builder.Build().RunAsync();
