using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manifold.Api.Data.Entities;

public class Bucket
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid Id { get; set; }
    
    [MaxLength(20)]
    [Required]
    public string DriverType { get; set; } = string.Empty;
    
    [MaxLength(128)]
    [Required]
    public string Name { get; set; } = string.Empty;
    [MaxLength(256)]
    public string Description { get; set; } = string.Empty;
    [MaxLength(2048)]
    [Required]
    public string ConnectionString { get; set; } = string.Empty;
}