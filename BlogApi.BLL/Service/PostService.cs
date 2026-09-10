using System.Text.RegularExpressions;
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
        // GET ALL
        // ==========================================

        public async Task<IEnumerable<PostDto>> GetAllAsync()
        {
            var posts =
                await _postRepository.GetAllAsync();

            return posts
                .Where(p => p.IsPublished)
                .Select(MapToDto);
        }

        // ==========================================
        // GET BY ID
        // ==========================================

        public async Task<PostDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var post =
                await _postRepository.GetByIdAsync(id);

            if (post == null ||
                !post.IsPublished)
            {
                return null;
            }

            return MapToDto(post);
        }

        // ==========================================
        // GET BY SLUG
        // ==========================================

        public async Task<PostDto?> GetBySlugAsync(
            string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return null;
            }

            slug = slug
                .Trim()
                .ToLowerInvariant();

            var post =
                await _postRepository
                    .GetBySlugAsync(slug);

            if (post == null ||
                !post.IsPublished)
            {
                return null;
            }

            return MapToDto(post);
        }

        // ==========================================
        // CREATE
        // ==========================================

        public async Task<PostDto> CreateAsync(
            CreatePostDto dto,
            int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException(
                    "Invalid user ID.");
            }

            if (dto == null)
            {
                throw new ArgumentNullException(
                    nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new ArgumentException(
                    "Post title is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                throw new ArgumentException(
                    "Post content is required.");
            }

            if (dto.CategoryId <= 0)
            {
                throw new ArgumentException(
                    "Valid category ID is required.");
            }

            // Check category
            var category =
                await _categoryRepository
                    .GetByIdAsync(dto.CategoryId);

            if (category == null)
            {
                throw new ArgumentException(
                    "Category not found.");
            }

            var title = dto.Title.Trim();

            var content = dto.Content.Trim();

            // Generate unique slug
            var slug =
                await CreateUniqueSlugAsync(title);

            var post = new Post
            {
                Title = title,

                Content = content,

                UserId = userId,

                CategoryId = dto.CategoryId,

                CreatedAt = DateTime.UtcNow,

                UpdatedAt = null,

                // Automatic publish
                IsPublished = true,

                Slug = slug
            };

            var createdPost =
                await _postRepository
                    .AddAsync(post);

            return MapToDto(createdPost);
        }

        // ==========================================
        // UPDATE
        // ==========================================

        public async Task<bool> UpdateAsync(
            int id,
            UpdatePostDto dto,
            int userId,
            bool isAdmin = false)
        {
            if (id <= 0 ||
                userId <= 0 ||
                dto == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(
                dto.Title))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(
                dto.Content))
            {
                return false;
            }

            if (dto.CategoryId <= 0)
            {
                return false;
            }

            var post =
                await _postRepository
                    .GetByIdAsync(id);

            if (post == null)
            {
                return false;
            }

            // Owner OR Admin
            if (post.UserId != userId &&
                !isAdmin)
            {
                return false;
            }

            // Check category
            var category =
                await _categoryRepository
                    .GetByIdAsync(
                        dto.CategoryId);

            if (category == null)
            {
                return false;
            }

            var title =
                dto.Title.Trim();

            var content =
                dto.Content.Trim();

            post.Title = title;

            post.Content = content;

            post.CategoryId =
                dto.CategoryId;

            // Generate unique slug
            post.Slug =
                await CreateUniqueSlugAsync(
                    title,
                    id);

            // Automatic publish
            post.IsPublished = true;

            post.UpdatedAt =
                DateTime.UtcNow;

            await _postRepository
                .UpdateAsync(post);

            return true;
        }

        // ==========================================
        // DELETE
        // ==========================================

        public async Task<bool> DeleteAsync(
            int id,
            int userId,
            bool isAdmin = false)
        {
            if (id <= 0 ||
                userId <= 0)
            {
                return false;
            }

            var post =
                await _postRepository
                    .GetByIdAsync(id);

            if (post == null)
            {
                return false;
            }

            // Owner OR Admin
            if (post.UserId != userId &&
                !isAdmin)
            {
                return false;
            }

            await _postRepository
                .DeleteAsync(id);

            return true;
        }

        // ==========================================
        // SEARCH
        // ==========================================

        public async Task<IEnumerable<PostDto>>
            SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Enumerable.Empty<PostDto>();
            }

            keyword = keyword.Trim();

            var posts =
                await _postRepository
                    .SearchAsync(keyword);

            return posts
                .Where(p => p.IsPublished)
                .Select(MapToDto);
        }

        // ==========================================
        // PAGINATION + SEARCH + FILTER
        // ==========================================

        public async Task<PostPagedResultDto>
            GetPagedAsync(
                int page,
                int pageSize,
                string? keyword,
                int? categoryId)
        {
            // Page
            if (page <= 0)
            {
                page = 1;
            }

            // Page size
            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            // Maximum
            if (pageSize > 100)
            {
                pageSize = 100;
            }

            // Category
            if (categoryId.HasValue &&
                categoryId.Value <= 0)
            {
                categoryId = null;
            }

            // Keyword
            if (!string.IsNullOrWhiteSpace(
                keyword))
            {
                keyword = keyword.Trim();
            }
            else
            {
                keyword = null;
            }

            // Count only published
            var totalItems =
                await _postRepository
                    .GetFilteredCountAsync(
                        keyword,
                        categoryId,
                        true);

            // Get only published
            var posts =
                await _postRepository
                    .GetFilteredPagedAsync(
                        keyword,
                        categoryId,
                        page,
                        pageSize,
                        true);

            var totalPages =
                totalItems == 0
                    ? 0
                    : (int)Math.Ceiling(
                        totalItems /
                        (double)pageSize);

            return new PostPagedResultDto
            {
                Items =
                    posts.Select(MapToDto),

                Page = page,

                PageSize = pageSize,

                TotalItems = totalItems,

                TotalPages = totalPages
            };
        }

        // ==========================================
        // UNIQUE SLUG
        // ==========================================

        private async Task<string>
            CreateUniqueSlugAsync(
                string title,
                int? excludePostId = null)
        {
            var baseSlug =
                GenerateSlug(title);

            var slug = baseSlug;

            var counter = 2;

            while (await _postRepository
                .ExistsBySlugAsync(
                    slug,
                    excludePostId))
            {
                slug =
                    $"{baseSlug}-{counter}";

                counter++;
            }

            return slug;
        }

        // ==========================================
        // SLUG GENERATOR
        // ==========================================

        private static string GenerateSlug(
            string title)
        {
            var slug =
                title
                    .Trim()
                    .ToLowerInvariant();

            // Remove special characters
            slug =
                Regex.Replace(
                    slug,
                    @"[^a-z0-9]+",
                    "-");

            return slug.Trim('-');
        }

        // ==========================================
        // ENTITY -> DTO
        // ==========================================

        private static PostDto MapToDto(
            Post post)
        {
            return new PostDto
            {
                Id = post.Id,

                Title = post.Title,

                Content = post.Content,

                Slug = post.Slug,

                IsPublished =
                    post.IsPublished,

                UserId =
                    post.UserId,

                CategoryId =
                    post.CategoryId,

                CreatedAt =
                    post.CreatedAt,

                UpdatedAt =
                    post.UpdatedAt,

                Author =
                    post.User == null
                        ? null
                        : new PostAuthorDto
                        {
                            Id =
                                post.User.User_id,

                            Username =
                                post.User.Username
                        },

                Category =
                    post.Category == null
                        ? null
                        : new PostCategoryDto
                        {
                            Id =
                                post.Category.Id,

                            Name =
                                post.Category.Name
                        }
            };
        }
    }
}