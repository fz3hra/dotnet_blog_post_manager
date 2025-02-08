namespace Post.API.ModelDtos;

public class PostsResponse : BaseResponse
{
    public List<GetPostDto> Posts { get; set; }
}