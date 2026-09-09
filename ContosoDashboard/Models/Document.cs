using System.ComponentModel.DataAnnotations;

namespace ContosoDashboard.Models;

public class Document
{
    [Key]
    public int DocumentId { get; set; }

    [Required]
    [MaxLength(256)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Tags { get; set; }

    public int UploaderId { get; set; }

    public DateTime UploadDateUtc { get; set; } = DateTime.UtcNow;

    public long FileSizeBytes { get; set; }

    [MaxLength(255)]
    public string ContentType { get; set; } = string.Empty;

    [Required]
    public string StoragePath { get; set; } = string.Empty;

    public int? ProjectId { get; set; }
}
