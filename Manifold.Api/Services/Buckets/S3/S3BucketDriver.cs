using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Manifold.Data.Entities;

namespace Manifold.Api.Services.Buckets.S3;

public class S3BucketDriver : IBucketDriver
{
    private readonly IAmazonS3 _client;
    private readonly TransferUtility _transferUtility;
    private readonly string _bucketName;
    
    /// <summary>
    /// Attempts to construct the connection client from provided connection string
    /// </summary>
    /// <param name="connectionString">JSON-serialized connection options</param>
    /// <exception cref="ArgumentException">connection string can't be parsed to S3Options</exception>
    /// <exception cref="JsonException">connection string is an invalid JSON</exception>
    public S3BucketDriver(string connectionString)
    {
        var options = JsonSerializer.Deserialize<S3Options>(connectionString, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        });
        if (options == null)
        {
            throw new ArgumentException("Invalid connection string", nameof(connectionString));
        }

        var config = new AmazonS3Config
        {
            ServiceURL = options.ServiceURL,
            AuthenticationRegion = options.BucketRegion,
            ForcePathStyle = options.ForcePathStyle,
        };
        
        Console.WriteLine($"Connecting to {options.ServiceURL} in {options.BucketRegion}");
        _client = new AmazonS3Client(options.AccessKeyId, options.SecretAccessKey, config);
        _transferUtility = new TransferUtility(_client);
        _bucketName = options.BucketName;
    }
    
    public async Task UploadAsync(FileMeta meta, Stream content)
    {
        var request = new TransferUtilityUploadRequest()
        {
            BucketName = _bucketName,
            Key = meta.Id.ToString(),
            InputStream = content,
            ContentType = meta.ContentType,
            AutoCloseStream = true,
        };
        
        await _transferUtility.UploadAsync(request);
    }

    public async Task DownloadAsync(string key, Stream stream)
    {
        using (var s3Stream = await _transferUtility.OpenStreamAsync(_bucketName, key))
        {
            await s3Stream.CopyToAsync(stream);
        }
    }

    public async Task DeleteAsync(string key)
    {
        var request = new DeleteObjectRequest()
        {
            BucketName = _bucketName,
            Key = key
        };
        
        await _client.DeleteObjectAsync(request);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            await _client.GetObjectMetadataAsync(_bucketName, key);
            return true;
        }
        catch
        {
            return false;
        }
    }
}