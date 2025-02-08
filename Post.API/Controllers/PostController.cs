using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Post.API.Contracts;
using Post.API.Data;
using Post.API.ModelDtos;

namespace Post.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostController : ControllerBase
{
    private readonly IPostRepository _postRepository;
    private readonly ILogger<PostController> _logger;

    public PostController(IPostRepository postRepository, ILogger<PostController> logger)
    {
        _postRepository = postRepository;
        _logger = logger;
    }

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