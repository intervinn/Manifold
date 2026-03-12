using System.ComponentModel.DataAnnotations;

namespace Manifold.Api.DTO;

[Serializable]
public class UploadFileForm
{
    [Required]
    public required IFormFile File { get; set; }
    [Required]
    public Guid DestinationBucket { get; set; }
}