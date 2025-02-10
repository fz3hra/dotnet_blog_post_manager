using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Post.API.Contracts;
using Post.API.Data;
using Post.API.ModelDtos;

namespace Post.API.Controllers;

/// <summary>
/// Controller to manage posts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostController : ControllerBase
{
    private readonly IPostRepository _postRepository;
    private readonly ILogger<PostController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostController"/> class.
    /// </summary>
    /// <param name="postRepository">The repository for managing posts.</param>
    /// <param name="logger">The logger instance for logging information.</param>
    public PostController(IPostRepository postRepository, ILogger<PostController> logger)
    {
        _postRepository = postRepository;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new post.
    /// </summary>
    /// <param name="postDto">The post data transfer object.</param>
    /// <returns>The response containing the created post.</returns>
    [HttpPost]
    public async Task<ActionResult<PostResponse>> CreatePost([FromBody] CreatePostDto postDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new BaseResponse 
                { 
                    Success = false, 
                    Message = "User not authenticated" 
                });
            }

            _logger.LogInformation("Authenticated User ID: {UserId}", userId);
            var result = await _postRepository.CreatePost(postDto, userId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating post");
            return StatusCode(500, new BaseResponse
            {
                Success = false,
                Message = "An error occurred while creating the post"
            });
        }
    }

    /// <summary>
    /// Retrieves all posts for the authenticated user.
    /// </summary>
    /// <param name="status">The optional status filter for the posts.</param>
    /// <returns>A list of posts matching the criteria.</returns>
    [HttpGet]
    public async Task<ActionResult<PostsResponse>> GetAllPosts([FromQuery] PostStatus? status)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _postRepository.GetAllPosts(userId, status);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving posts");
            return StatusCode(500, new PostsResponse
            {
                Success = false,
                Message = "An error occurred while retrieving posts"
            });
        }
    }

    /// <summary>
    /// Retrieves a specific post by its ID.
    /// </summary>
    /// <param name="id">The ID of the post to retrieve.</param>
    /// <returns>The requested post, if found.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<PostResponse>> GetPost(int id)
    {
        try
        {
            var result = await _postRepository.GetPostById(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving post {PostId}", id);
            return StatusCode(500, new PostResponse
            {
                Success = false,
                Message = "An error occurred while retrieving the post"
            });
        }
    }

    /// <summary>
    /// Updates an existing post.
    /// </summary>
    /// <param name="id">The ID of the post to update.</param>
    /// <param name="postDto">The updated post data.</param>
    /// <returns>The response containing the updated post.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<PostResponse>> UpdatePost(int id, [FromBody] UpdatePostDto postDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new BaseResponse 
                { 
                    Success = false, 
                    Message = "User not authenticated" 
                });
            }

            var result = await _postRepository.UpdatePost(id, postDto, userId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating post {PostId}", id);
            return StatusCode(500, new PostResponse
            {
                Success = false,
                Message = "An error occurred while updating the post"
            });
        }
    }

    /// <summary>
    /// Deletes a post.
    /// </summary>
    /// <param name="id">The ID of the post to delete.</param>
    /// <returns>The response indicating the deletion status.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<BaseResponse>> DeletePost(int id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new BaseResponse 
                { 
                    Success = false, 
                    Message = "User not authenticated" 
                });
            }

            var result = await _postRepository.DeletePost(id, userId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting post {PostId}", id);
            return StatusCode(500, new BaseResponse
            {
                Success = false,
                Message = "An error occurred while deleting the post"
            });
        }
    }
}
