using AutoMapper;
using Post.API.Contracts;
using Microsoft.EntityFrameworkCore;
using Post.API.Contracts;
using Post.API.Data;
using Post.API.ModelDtos;

namespace Post.API.Repository;

public class PostRepository : IPostRepository
{
    private readonly PostDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<PostRepository> _logger;

    public PostRepository(PostDbContext context, IMapper mapper, ILogger<PostRepository> logger)
    {
        this._context = context;
        this._mapper = mapper;
        this._logger = logger;
    }

    public async Task<PostResponse> CreatePost(CreatePostDto postDto, string userId)
    {
        try
        {
            var post = _mapper.Map<Data.Post>(postDto);
            post.CreatedBy = userId;
            post.CreatedAt = DateTime.UtcNow;

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<GetPostDetailDto>(post);
            return new PostResponse
            {
                Success = true,
                Message = "Post created successfully",
                Post = responseDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating post");
            return new PostResponse
            {
                Success = false,
                Message = $"Failed to create post: {ex.Message}" // 🔍 Show actual error
            };
        }

    }

    public async Task<PostsResponse> GetAllPosts(string? userId = null, PostStatus? status = null)
    {
        try
        {
            var query = _context.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(p => p.CreatedBy == userId);
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            var posts = await query.ToListAsync();
            var postsDto = _mapper.Map<List<GetPostDto>>(posts);

            return new PostsResponse
            {
                Success = true,
                Posts = postsDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving posts");
            return new PostsResponse
            {
                Success = false,
                Message = "Failed to retrieve posts"
            };
        }
    }

    public async Task<PostResponse> GetPostById(int id)
    {
        try
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return new PostResponse
                {
                    Success = false,
                    Message = "Post not found"
                };
            }

            var postDto = _mapper.Map<GetPostDetailDto>(post);
            return new PostResponse
            {
                Success = true,
                Post = postDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving post {PostId}", id);
            return new PostResponse
            {
                Success = false,
                Message = "Failed to retrieve post"
            };
        }
    }

    public async Task<PostResponse> UpdatePost(int id, UpdatePostDto postDto, string userId)
    {
        try
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return new PostResponse
                {
                    Success = false,
                    Message = "Post not found"
                };
            }

            if (post.CreatedBy != userId)
            {
                return new PostResponse
                {
                    Success = false,
                    Message = "Unauthorized to update this post"
                };
            }

            _mapper.Map(postDto, post);
            post.LastModifiedAt = DateTime.UtcNow;
            post.LastModifiedBy = userId;

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            var updatedPostDto = _mapper.Map<GetPostDetailDto>(post);
            return new PostResponse
            {
                Success = true,
                Message = "Post updated successfully",
                Post = updatedPostDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating post {PostId}", id);
            return new PostResponse
            {
                Success = false,
                Message = "Failed to update post"
            };
        }
    }

    public async Task<BaseResponse> DeletePost(int id, string userId)
    {
        try
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "Post not found"
                };
            }

            if (post.CreatedBy != userId)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "Unauthorized to delete this post"
                };
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return new BaseResponse
            {
                Success = true,
                Message = "Post deleted successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting post {PostId}", id);
            return new BaseResponse
            {
                Success = false,
                Message = "Failed to delete post"
            };
        }
    }

    public async Task<PostsResponse> GetPostsByTag(string tag)
    {
        try
        {
            var posts = await _context.Posts
                .Where(p => p.Tags.Contains(tag))
                .ToListAsync();

            var postsDto = _mapper.Map<List<GetPostDto>>(posts);
            return new PostsResponse
            {
                Success = true,
                Posts = postsDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving posts by tag {Tag}", tag);
            return new PostsResponse
            {
                Success = false,
                Message = "Failed to retrieve posts by tag"
            };
        }
    }

    public async Task<bool> IncrementViewCount(int id)
    {
        try
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return false;

            post.ViewCount++;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while incrementing view count for post {PostId}", id);
            return false;
        }
    }

    public Task<PostsResponse> SearchPosts(string searchTerm)
    {
        throw new NotImplementedException();
    }
}