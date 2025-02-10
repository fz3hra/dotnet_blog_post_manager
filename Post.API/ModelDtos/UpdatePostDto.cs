using Post.API.Data;

namespace Post.API.ModelDtos;

public class UpdatePostDto : BasePostDto
{
    public bool IsPublished { get; set; } = false;
    public PostStatus Status { get; set; }
}