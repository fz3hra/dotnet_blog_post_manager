using System.ComponentModel.DataAnnotations;

namespace Post.API.Data;

public class Post
{
    public int Id { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string CreatedBy { get; set; } 

    public DateTime? LastModifiedAt { get; set; }

    public string? LastModifiedBy { get; set; }

    public bool IsPublished { get; set; } = false;

    public DateTime? PublishedAt { get; set; }

    [MaxLength(10)]
    public List<string> Tags { get; set; } = new();

    public string? FeaturedImageUrl { get; set; }

    public int ViewCount { get; set; } = 0;

    [StringLength(200)]
    public string? Excerpt { get; set; } 

    public PostStatus Status { get; set; } = PostStatus.Draft;
}

public enum PostStatus
{
    Draft,
    Published,
    Archived
}