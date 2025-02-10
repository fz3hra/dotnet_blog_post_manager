using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Post.API.Contracts;
using Post.API.Controllers;
using Post.API.Data;
using Post.API.ModelDtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Post.API.Tests.Controllers
{
    public class PostControllerTests
    {
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly Mock<ILogger<PostController>> _mockLogger;
        private readonly PostController _controller;

        public PostControllerTests()
        {
            _mockPostRepository = new Mock<IPostRepository>();
            _mockLogger = new Mock<ILogger<PostController>>();
            _controller = new PostController(_mockPostRepository.Object, _mockLogger.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task CreatePost_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var postDto = new CreatePostDto
            {
                Title = "Test Post",
                Description = "Test Content"
            };

            var expectedResponse = new PostResponse
            {
                Success = true,
                Message = "Post created successfully",
                Post = new GetPostDetailDto { Id = 1, Title = "Test Post" }
            };

            _mockPostRepository.Setup(x => x.CreatePost(postDto, "test-user-id"))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.CreatePost(postDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<PostResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(expectedResponse.Message, response.Message);
        }

        [Fact]
        public async Task CreatePost_Unauthorized_ReturnsUnauthorizedResult()
        {
            // Arrange
            var postDto = new CreatePostDto { Title = "Test Post" };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            // Act
            var result = await _controller.CreatePost(postDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var response = Assert.IsType<BaseResponse>(unauthorizedResult.Value);
            Assert.False(response.Success);
            Assert.Equal("User not authenticated", response.Message);
        }

        [Fact]
        public async Task GetAllPosts_ReturnsOkResult()
        {
            // Arrange
            var expectedResponse = new PostsResponse
            {
                Success = true,
                Posts = new List<GetPostDto>
                {
                    new() { Id = 1, Title = "Test Post 1" },
                    new() { Id = 2, Title = "Test Post 2" }
                }
            };

            _mockPostRepository.Setup(x => x.GetAllPosts("test-user-id", null))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetAllPosts(null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<PostsResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(2, response.Posts.Count);
        }

        [Fact]
        public async Task GetPost_ExistingId_ReturnsOkResult()
        {
            // Arrange
            int postId = 1;
            var expectedResponse = new PostResponse
            {
                Success = true,
                Post = new GetPostDetailDto { Id = postId, Title = "Test Post" }
            };

            _mockPostRepository.Setup(x => x.GetPostById(postId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetPost(postId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<PostResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(postId, response.Post.Id);
        }

        [Fact]
        public async Task GetPost_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            int postId = 999;
            var expectedResponse = new PostResponse
            {
                Success = false,
                Message = "Post not found"
            };

            _mockPostRepository.Setup(x => x.GetPostById(postId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetPost(postId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<PostResponse>(notFoundResult.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task UpdatePost_ValidInput_ReturnsOkResult()
        {
            // Arrange
            int postId = 1;
            var updateDto = new UpdatePostDto
            {
                Title = "Updated Title",
                Description = "Updated Content"
            };

            var expectedResponse = new PostResponse
            {
                Success = true,
                Message = "Post updated successfully",
                Post = new GetPostDetailDto { Id = postId, Title = "Updated Title" }
            };

            _mockPostRepository.Setup(x => x.UpdatePost(postId, updateDto, "test-user-id"))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UpdatePost(postId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<PostResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Post updated successfully", response.Message);
        }

        [Fact]
        public async Task DeletePost_ExistingId_ReturnsOkResult()
        {
            // Arrange
            int postId = 1;
            var expectedResponse = new BaseResponse
            {
                Success = true,
                Message = "Post deleted successfully"
            };

            _mockPostRepository.Setup(x => x.DeletePost(postId, "test-user-id"))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.DeletePost(postId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<BaseResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Post deleted successfully", response.Message);
        }
    }
}