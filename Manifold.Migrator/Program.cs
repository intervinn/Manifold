using Manifold.Api.Data;
using Manifold.Migrator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.ConfigureWarnings(w => w.Ignore(RelationalEventId.MigrationsUserTransactionWarning));
    options.UseNpgsql(builder.Configuration.GetConnectionString("Manifold"), x =>
    {
        x.MigrationsAssembly("Manifold.Api");
    });
});

var host = builder.Build();
host.Run();

