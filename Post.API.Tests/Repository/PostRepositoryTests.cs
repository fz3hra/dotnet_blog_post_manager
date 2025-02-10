using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Post.API.Data;
using Post.API.ModelDtos;
using Post.API.Repository;
using Xunit;

namespace Post.API.Tests.Repository
{
    public class PostRepositoryTests
    {
        private readonly PostDbContext _context;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<PostRepository>> _mockLogger;
        private readonly PostRepository _repository;

        public PostRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<PostDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PostDbContext(options);
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<PostRepository>>();
            _repository = new PostRepository(_context, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreatePost_ValidInput_ReturnsSuccessResponse()
        {
            // Arrange
            var createDto = new CreatePostDto
            {
                Title = "Test Post",
                Description = "Test Content"
            };

            var post = new Data.Post
            {
                Title = createDto.Title,
                Description = createDto.Description
            };

            var postDetailDto = new GetPostDetailDto
            {
                Id = 1,
                Title = createDto.Title,
                Description = createDto.Description
            };

            _mockMapper.Setup(x => x.Map<Data.Post>(createDto)).Returns(post);
            _mockMapper.Setup(x => x.Map<GetPostDetailDto>(It.IsAny<Data.Post>()))
                .Returns(postDetailDto);

            // Act
            var result = await _repository.CreatePost(createDto, "test-user-id");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Post created successfully", result.Message);
            Assert.NotNull(result.Post);
            Assert.Equal(createDto.Title, result.Post.Title);
        }

        [Fact]
        public async Task GetAllPosts_ReturnsAllPosts()
        {
            // Arrange
            var posts = new List<Data.Post>
            {
                new() {
                    Id = 1,
                    Title = "Post 1",
                    Description = "Description 1",
                    CreatedBy = "user1",
                    CreatedAt = DateTime.UtcNow,
                    Status = PostStatus.Published
                },
                new() {
                    Id = 2,
                    Title = "Post 2",
                    Description = "Description 2",
                    CreatedBy = "user1",
                    CreatedAt = DateTime.UtcNow,
                    Status = PostStatus.Published
                }
            };

            await _context.Posts.AddRangeAsync(posts);
            await _context.SaveChangesAsync();

            var postDtos = posts.Select(p => new GetPostDto
            {
                Id = p.Id,
                Title = p.Title
            }).ToList();

            _mockMapper.Setup(x => x.Map<List<GetPostDto>>(It.IsAny<List<Data.Post>>()))
                .Returns(postDtos);

            // Act
            var result = await _repository.GetAllPosts("user1");

            // Assert
            Assert.True(result.Success);
            Assert.Equal(2, result.Posts.Count);
        }

        [Fact]
        public async Task GetPostById_ExistingPost_ReturnsPost()
        {
            // Arrange
            var post = new Data.Post
            {
                Id = 1,
                Title = "Test Post",
                Description = "Test Description",
                CreatedBy = "test-user-id",
                CreatedAt = DateTime.UtcNow,
                Status = PostStatus.Published
            };

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            var postDetailDto = new GetPostDetailDto
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description
            };

            _mockMapper.Setup(x => x.Map<GetPostDetailDto>(It.IsAny<Data.Post>()))
                .Returns(postDetailDto);

            // Act
            var result = await _repository.GetPostById(1);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Post);
            Assert.Equal(post.Title, result.Post.Title);
        }

        [Fact]
        public async Task UpdatePost_ValidInput_ReturnsUpdatedPost()
        {
            // Arrange
            var post = new Data.Post
            {
                Id = 1,
                Title = "Original Title",
                Description = "Original Content",
                CreatedBy = "test-user-id"
            };

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            var updateDto = new UpdatePostDto
            {
                Title = "Updated Title",
                Description = "Updated Content"
            };

            var updatedPostDto = new GetPostDetailDto
            {
                Id = 1,
                Title = updateDto.Title,
                Description = updateDto.Description
            };

            _mockMapper.Setup(x => x.Map(updateDto, post));
            _mockMapper.Setup(x => x.Map<GetPostDetailDto>(It.IsAny<Data.Post>()))
                .Returns(updatedPostDto);

            // Act
            var result = await _repository.UpdatePost(1, updateDto, "test-user-id");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Post updated successfully", result.Message);
            Assert.Equal(updateDto.Title, result.Post.Title);
        }

        [Fact]
        public async Task DeletePost_ExistingPost_ReturnsSuccess()
        {
            // Arrange
            var post = new Data.Post
            {
                Id = 1,
                Title = "Test Post",
                Description = "Test Description",
                CreatedBy = "test-user-id",
                CreatedAt = DateTime.UtcNow,
                Status = PostStatus.Published
            };

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeletePost(1, "test-user-id");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Post deleted successfully", result.Message);
            Assert.Empty(await _context.Posts.ToListAsync());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}