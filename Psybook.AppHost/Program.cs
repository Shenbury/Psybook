var builder = DistributedApplication.CreateBuilder(args);

var databaseName = "wmsp-db";

var sql = builder.AddSqlServer("sql", port: 14329)
                 .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase(databaseName);

var migrations = builder.AddProject<Projects.Psybook_Migrations>("migrations")
    .WithReference(sql)
    .WaitFor(sql);

builder.AddProject<Projects.Psybook_API>("psybook-api")
    .WithReference(db)
    .WithReference(migrations)
       .WaitFor(db)
       .WaitForCompletion(migrations);

builder.AddProject<Projects.Psybook_UI>("psybook-ui");
builder.AddProject<Projects.Psybook_UI_Client>("psybook-ui-client");

builder.AddProject<Projects.Psybook_Migrations>("psybook-migrations")
.WithReference(db)
.WithReference(migrations)
.WaitFor(db)
.WaitForCompletion(migrations);

builder.Build().Run();
