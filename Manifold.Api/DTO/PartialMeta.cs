using Manifold.Data.Entities;

namespace Manifold.Api.DTO;

[Serializable]
public sealed class PartialMeta
{
    public Guid Id { get; set; }
    public string Filename { get; set; } = string.Empty;
    public required string ContentType { get; set; } = string.Empty;
    public long ContentLength { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public static PartialMeta From(FileMeta meta)
    {
        return new PartialMeta()
        {
            Id = meta.Id,
            Filename = meta.Filename,
            ContentType = meta.ContentType,
            ContentLength = meta.ContentLength,
            CreatedAt = meta.CreatedAt
        };
    }
}