using Post.API.Data;
using Post.API.ModelDtos;

namespace Post.API.Contracts;

public interface IPostRepository
{
    Task<PostResponse> CreatePost(CreatePostDto postDto, string userId);

    Task<PostsResponse> GetAllPosts(string? userId = null, PostStatus? status = null);

    Task<PostResponse> GetPostById(int id);

    Task<PostResponse> UpdatePost(int id, UpdatePostDto postDto, string userId);

    Task<BaseResponse> DeletePost(int id, string userId);

    Task<PostsResponse> GetPostsByTag(string tag);
    Task<bool> IncrementViewCount(int id);
    Task<PostsResponse> SearchPosts(string searchTerm);
}