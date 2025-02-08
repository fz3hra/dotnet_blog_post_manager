using Post.API.Data;

namespace Post.API.ModelDtos;

public class GetPostDto : BasePostDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public PostStatus Status { get; set; }
    public int ViewCount { get; set; }
}