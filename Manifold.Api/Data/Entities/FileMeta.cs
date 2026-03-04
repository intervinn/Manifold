using System.Net.Mime;

namespace Manifold.Api.Data.Entities;

public class FileMeta
{
    public Guid Id { get; set; }
    public required string Filename { get; set; }
    public required ContentType ContentType { get; set; }
    public long ContentLength { get; set; }
    public DateTime CreatedAt { get; set; }
}