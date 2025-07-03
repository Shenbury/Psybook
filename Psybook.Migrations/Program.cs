using Psybook.Migrations;
using Psybook.Shared.Contexts;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddSqlServerDbContext<BookingContext>(connectionName: "wmsp-db");

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
