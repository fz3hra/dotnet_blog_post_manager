using System.ComponentModel.DataAnnotations;

namespace Post.API.ModelDtos;

public abstract class BasePostDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    public List<string>? Tags { get; set; }

    public string? FeaturedImageUrl { get; set; }

    public string? Excerpt { get; set; }
}