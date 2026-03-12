using System.ComponentModel.DataAnnotations;

namespace Manifold.Api.DTO;

[Serializable]
public class CreateBucketBody
{
    [Required]
    public string DriverType { get; set; } = string.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
}