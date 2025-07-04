using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using MudBlazor.Services;
using Psybook.ServiceDefaults;
using Psybook.Services.UI.Clients;
using Psybook.Services.UI.DataLoaders;
using Psybook.Shared.Communication;
using Psybook.Shared.Extensions;
using Psybook.UI.Components;
using Psybook.UI.Renderers;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire Defaults
builder.AddServiceDefaults();

// Add Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi()
            .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
            .AddInMemoryTokenCaches();

// Add MudBlazor services
builder.Services.AddMudServices();

builder.Services.ClientAndServerRegistrations();

// RenderContext communicates to components in which RenderMode the component is running.
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IRenderContext, ServerRenderContext>();

builder.Services.AddHttpClient("psybook-api", https => https.BaseAddress = new Uri("https://psybook-api")).AddServiceDiscovery();

// Data Loader Service
builder.Services.AddScoped<IBookingLoaderService, BookingDataLoaderService>();
builder.Services.AddScoped<BookingClient>();

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


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Psybook.UI.Client._Imports).Assembly);

app.Run();
