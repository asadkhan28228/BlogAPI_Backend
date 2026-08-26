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

            return posts.Select(MapToDto);
        }

        public async Task<PostDto?> GetByIdAsync(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);

            if (post == null)
            {
                return null;

            }

            return MapToDto(post);
        }

        public async Task<PostDto> CreateAsync(CreatePostDto dto, int userId)
        {
            var post = new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                Userid = userId,
                CreatedAt = DateTime.UtcNow
            };

            var createdPost = await _postRepository.AddAsync(post);

            return MapToDto(createdPost);
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdatePostDto dto,
            int userId)
        {
            var post = await _postRepository.GetByIdAsync(id);

            if (post == null)
                return false;

            // User can update only his own post
            if (post.Userid != userId)
                return false;

            post.Title = dto.Title;
            post.Content = dto.Content;
            post.UpdatedAt = DateTime.UtcNow;

            await _postRepository.UpdateAsync(post);

            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var post = await _postRepository.GetByIdAsync(id);

            if (post == null)
                return false;

            // User can delete only his own post
            if (post.Userid != userId)
                return false;

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
                UserId = post.Userid,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };
        }
    }
}