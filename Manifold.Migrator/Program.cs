using Manifold.Data;
using Manifold.Migrator;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Manifold"), x =>
    {
        x.MigrationsAssembly("Manifold.Data");
    });
});

var host = builder.Build();
host.Run();

