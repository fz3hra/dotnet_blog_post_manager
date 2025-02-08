namespace Post.API.ModelDtos;

public class GetPostDetailDto : GetPostDto
{
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}