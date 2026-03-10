using System.Collections.Concurrent;
using Manifold.Api.Data;
using Manifold.Api.Data.Entities;
using Manifold.Api.Services.Buckets.S3;
using Microsoft.EntityFrameworkCore;

namespace Manifold.Api.Services.Buckets;

/// <summary>
/// Manages drivers at runtime, loads and dumps configurations into db
/// </summary>
public class BucketService : IHostedService
{
    private readonly ConcurrentDictionary<Guid, IBucketDriver> _drivers = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BucketService> _logger;
    
    public BucketService(IServiceProvider serviceProvider, ILogger<BucketService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public IBucketDriver? GetDriver(Guid bucketId) 
        => _drivers.GetValueOrDefault(bucketId);

    public void UpdateDriver(Guid bucketId, IBucketDriver driver)
        => _drivers.AddOrUpdate(bucketId, _ => _drivers[bucketId], (k, v) => v);

    public void UpdateDriver(Bucket bucket)
        => _drivers.AddOrUpdate(bucket.Id, _ => CreateDriver(bucket), (k, v) => v);
    
    public void RemoveDriver(Guid bucketId)
        => _drivers.TryRemove(bucketId, out _);

    private IBucketDriver CreateDriver(Bucket bucket)
    => bucket.DriverType switch
        {
            "s3" => new S3BucketDriver(bucket.ConnectionString),
            _ => throw new ArgumentException("Invalid driver Type")
        };
    

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var drivers = await dbContext.Buckets.ToListAsync(cancellationToken);
        foreach (var driver in drivers)
        {
            _logger.LogInformation($"Creating driver {driver.Name}");
            try
            {
                var instance = CreateDriver(driver);
                _drivers.TryAdd(driver.Id, instance);
                _logger.LogInformation($"Driver {driver.Name} created");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating driver");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
       return Task.CompletedTask;
    }
}