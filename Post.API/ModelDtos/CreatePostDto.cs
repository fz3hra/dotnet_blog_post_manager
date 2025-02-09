namespace Post.API.ModelDtos;

public class CreatePostDto : BasePostDto
{
    public bool IsPublished { get; set; } = false;
}