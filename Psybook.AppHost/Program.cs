var builder = DistributedApplication.CreateBuilder(args);

var databaseName = "wmsp-db";

var sql = builder.AddSqlServer("sql", port: 14329)
                 .WithEndpoint(name: "sqlEndpoint", targetPort: 14330)
                 .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase(databaseName);

var migrations = builder.AddProject<Projects.Psybook_Migrations>("psybook-migrations")
    .WithReference(sql)
    .WithReference(db)
    .WaitFor(db)
    .WaitFor(sql);

builder.AddProject<Projects.Psybook_API>("psybook-api")
    .WithReference(db)
    .WithReference(migrations)
       .WaitFor(db)
       .WaitForCompletion(migrations);

builder.AddProject<Projects.Psybook_UI>("psybook-ui");
builder.AddProject<Projects.Psybook_UI_Client>("psybook-ui-client");

builder.Build().Run();
