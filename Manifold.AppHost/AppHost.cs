var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("Postgres");
var database = postgres.AddDatabase("Manifold");

var migrator = builder.AddProject<Projects.Manifold_Migrator>("Migrator")
    .WithReference(database)
    .WaitFor(postgres);

builder.AddProject<Projects.Manifold_Api>("Api")
    .WithReference(database)
    .WithReference(migrator)
    .WaitForCompletion(migrator);

builder.Build().Run();
