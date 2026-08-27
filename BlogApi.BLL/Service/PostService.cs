using BlogApi.BLL.DTOs.Post;
using BlogApi.BLL.Interfaces;
using BlogApi.DAL.Entities;
using BlogApi.DAL.InterfacesRepositories;

namespace BlogApi.BLL.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<IEnumerable<PostDto>> GetAllAsync()
        {
            var posts = await _postRepository.GetAllAsync();

            if (posts == null || !posts.Any())
            {
                return Enumerable.Empty<PostDto>();
            }

            return posts.Select(MapToDto);
        }

        public async Task<PostDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var post = await _postRepository.GetByIdAsync(id);

            if (post == null)
            {
                return null;
            }

            return MapToDto(post);
        }

        public async Task<PostDto> CreateAsync(CreatePostDto dto, int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid user ID.");
            }

            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new ArgumentException("Post title is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                throw new ArgumentException("Post content is required.");
            }

            var post = new Post
            {
                Title = dto.Title.Trim(),
                Content = dto.Content.Trim(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsPublished = false,
                Slug = dto.Title
                    .Trim()
                    .ToLower()
                    .Replace(" ", "-")
            };

            var createdPost = await _postRepository.AddAsync(post);

            return MapToDto(createdPost);
        }

        public async Task<bool> UpdateAsync(int id, UpdatePostDto dto, int userId)
        {

            if (dto == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return false;
            }

            var post = await _postRepository.GetByIdAsync(id);

            if (post == null)
            {
                return false;
            }

            if (post.UserId != userId)
            {
                return false;
            }

            post.Title = dto.Title.Trim();
            post.Content = dto.Content.Trim();
            post.UpdatedAt = DateTime.UtcNow;

            await _postRepository.UpdateAsync(post);

            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            if (id <= 0)
            {
                return false;
            }

            if (userId <= 0)
            {
                return false;
            }

            var post = await _postRepository.GetByIdAsync(id);

            if (post == null)
            {
                return false;
            }

            if (post.UserId != userId)
            {
                return false;
            }

            await _postRepository.DeleteAsync(id);

            return true;
        }

        private static PostDto MapToDto(Post post)
        {
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                UserId = post.UserId,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };
        }
    }
}