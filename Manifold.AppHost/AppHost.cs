var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("env");

var postgres = builder.AddPostgres("Postgres")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("Manifold");

var migrator = builder.AddProject<Projects.Manifold_Migrator>("Migrator")
    .WithReference(database)
    .WaitFor(postgres);

var api = builder.AddProject<Projects.Manifold_Api>("Api")
    .WithReference(database)
    .WithReference(migrator)
    .WaitForCompletion(migrator);


builder.AddViteApp("App", "../Manifold.App")
    .WithEnvironment("VITE_API_URL", api.GetEndpoint("https"))
    .WithReference(api)
    .WithExternalHttpEndpoints()
    .WithPnpm()
    .PublishAsDockerFile();

var app = builder.Build();
app.Run();