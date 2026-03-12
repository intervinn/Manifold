using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manifold.Data.Entities;

public class FileMeta
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid Id { get; set; }
    [MaxLength(1024)]
    public required string Filename { get; set; }
    
    [MaxLength(128)]
    public required string ContentType { get; set; }
    public long ContentLength { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required Bucket Bucket { get; set; }
}