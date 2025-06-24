var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Psybook_API>("psybook-api");
builder.AddProject<Projects.Psybook_UI>("psybook-ui");
builder.AddProject<Projects.Psybook_UI_Client>("psybook-ui-client");

builder.Build().Run();
