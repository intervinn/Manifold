namespace Manifold.Api.Services.Buckets.S3;

public sealed class S3Options
{
    public string ServiceURL { get; set; } = string.Empty;
    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string BucketRegion { get; set; } = string.Empty;
    public bool ForcePathStyle { get; set; }
}