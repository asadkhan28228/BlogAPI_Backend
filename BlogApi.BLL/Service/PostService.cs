using BlogApi.BLL.DTOs.Post;
using BlogApi.BLL.Interfaces;
using BlogApi.DAL.Entities;
using BlogApi.DAL.InterfacesRepositories;

namespace BlogApi.BLL.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;

        private readonly ICategoryRepository _categoryRepository;


        public PostService(
            IPostRepository postRepository,
            ICategoryRepository categoryRepository)
        {
            _postRepository = postRepository;

            _categoryRepository = categoryRepository;
        }


        // ==========================================
        // GET ALL POSTS
        // ==========================================

        public async Task<IEnumerable<PostDto>> GetAllAsync()
        {
            var posts = await _postRepository.GetAllAsync();

            if (posts == null || !posts.Any())
            {
                return Enumerable.Empty<PostDto>();
            }

            return posts.Select(MapToDto);
        }


        // ==========================================
        // GET POST BY ID
        // ==========================================

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


        // ==========================================
        // CREATE POST
        // ==========================================

        public async Task<PostDto> CreateAsync(
            CreatePostDto dto,
            int userId)
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

            if (dto.CategoryId <= 0)
            {
                throw new ArgumentException("Valid category ID is required.");
            }


            // Check Category
            var category =await _categoryRepository.GetByIdAsync(dto.CategoryId);

            if (category == null)
            {
                throw new ArgumentException("Category not found.");
            }


            var title = dto.Title.Trim();

            var content = dto.Content.Trim();


            var post = new Post
            {
                Title = title,

                Content = content,

                UserId = userId,

                CategoryId = dto.CategoryId,

                CreatedAt = DateTime.UtcNow,

                UpdatedAt = null,

                IsPublished = false,

                Slug = GenerateSlug(title)
            };


            var createdPost =await _postRepository.AddAsync(post);
            return MapToDto(createdPost);
        }


        // ==========================================
        // UPDATE POST
        // ==========================================

        public async Task<bool> UpdateAsync(int id,UpdatePostDto dto,int userId)
        {
            if (id <= 0)
            {
                return false;
            }

            if (userId <= 0)
            {
                return false;
            }

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

            if (dto.CategoryId <= 0)
            {
                return false;
            }


            // Get Post
            var post =await _postRepository.GetByIdAsync(id);

            if (post == null)
            {
                return false;
            }


            // Only Owner Can Update
            if (post.UserId != userId)
            {
                return false;
            }


            // Check Category
            var category =await _categoryRepository.GetByIdAsync(dto.CategoryId);

            if (category == null)
            {
                return false;
            }


            var title = dto.Title.Trim();

            var content = dto.Content.Trim();


            // Update fields
            post.Title = title;

            post.Content = content;

            post.CategoryId = dto.CategoryId;

            post.Slug = GenerateSlug(title);

            post.UpdatedAt = DateTime.UtcNow;


            await _postRepository.UpdateAsync(post);


            return true;
        }


        // ==========================================
        // DELETE POST
        // ==========================================

        public async Task<bool> DeleteAsync(int id,int userId)
        {
            if (id <= 0)
            {
                return false;
            }

            if (userId <= 0)
            {
                return false;
            }


            // Get Post
            var post =
                await _postRepository.GetByIdAsync(id);

            if (post == null)
            {
                return false;
            }


            // Only Owner Can Delete
            if (post.UserId != userId)
            {
                return false;
            }


            await _postRepository.DeleteAsync(id);


            return true;
        }


        // ==========================================
        // MAP ENTITY -> DTO
        // ==========================================

        private static PostDto MapToDto(Post post)
        {
            return new PostDto
            {
                Id = post.Id,

                Title = post.Title,

                Content = post.Content,

                Slug = post.Slug,

                IsPublished = post.IsPublished,

                UserId = post.UserId,

                CategoryId = post.CategoryId,

                CreatedAt = post.CreatedAt

                
            };
        }


        // ==========================================
        // GENERATE SLUG
        // ==========================================

        private static string GenerateSlug(string title)
        {
            return title
                .Trim()
                .ToLower()
                .Replace(" ", "-");
        }

        // ==========================================
        // SEARCH POSTS
        // ==========================================

        public async Task<IEnumerable<PostDto>> SearchAsync(
            string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Enumerable.Empty<PostDto>();
            }

            keyword = keyword.Trim();

            var posts =
                await _postRepository.SearchAsync(keyword);

            if (posts == null || !posts.Any())
            {
                return Enumerable.Empty<PostDto>();
            }

            return posts.Select(MapToDto);
        }
    }
}