using Post.API.Data;

namespace Post.API.ModelDtos;

public class UpdatePostDto : BasePostDto
{
    public PostStatus Status { get; set; }
}