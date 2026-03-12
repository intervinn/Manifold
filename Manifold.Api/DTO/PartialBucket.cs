using Manifold.Data.Entities;

namespace Manifold.Api.DTO;

public class PartialBucket
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public static PartialBucket From(Bucket bucket)
    {
        return new PartialBucket
        {
            Id = bucket.Id,
            Name = bucket.Name,
            Description = bucket.Description
        };
    }
}