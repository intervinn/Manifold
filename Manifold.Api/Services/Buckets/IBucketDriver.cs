using Manifold.Data.Entities;

namespace Manifold.Api.Services.Buckets;

public interface IBucketDriver
{
    Task UploadAsync(FileMeta meta, Stream content);
    Task DownloadAsync(string key, Stream stream); 
    Task DeleteAsync(string key);
    Task<bool> ExistsAsync(string key); 
}

