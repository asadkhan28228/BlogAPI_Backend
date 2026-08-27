//using BlogApi.BLL.DTOs.Post;
//using BlogApi.BLL.Interfaces;
//using BlogApi.DAL.Entities;
//using BlogApi.DAL.InterfacesRepositories;

//namespace BlogApi.BLL.Services
//{
//    public class PostService : IPostService
//    {
//        private readonly IPostRepository _postRepository;

//        public PostService(IPostRepository postRepository)
//        {
//            _postRepository = postRepository;
//        }

//        //// ============================
//        // GET ALL POSTS
//        // ============================
//        public async Task<IEnumerable<PostDto>> GetAllAsync()
//        {
//            var posts = await _postRepository.GetAllAsync();

//            if (posts == null || !posts.Any())
//            {
//                return Enumerable.Empty<PostDto>();
//            }

//            return posts.Select(MapToDto);
//        }


//        // ============================
//        // GET POST BY ID
//        // ============================
//        public async Task<PostDto?> GetByIdAsync(int id)
//        {
//            // ID valid honi chahiye
//            if (id <= 0)
//            {
//                return null;
//            }

//            var post = await _postRepository.GetByIdAsync(id);

//            // Post nahi mila
//            if (post == null)
//            {
//                return null;
//            }

//            return MapToDto(post);
//        }


//        // ============================
//        // CREATE POST
//        // ============================
//        public async Task<PostDto> CreateAsync(
//            CreatePostDto dto,
//            int userId)
//        {
//            // User ID valid honi chahiye
//            if (userId <= 0)
//            {
//                throw new ArgumentException("Invalid user ID.");
//            }

//            // DTO null check
//            if (dto == null)
//            {
//                throw new ArgumentNullException(nameof(dto));
//            }

//            // Title validation
//            if (string.IsNullOrWhiteSpace(dto.Title))
//            {
//                throw new ArgumentException("Post title is required.");
//            }

//            // Content validation
//            if (string.IsNullOrWhiteSpace(dto.Content))
//            {
//                throw new ArgumentException("Post content is required.");
//            }

            

//            var post = new Post
//            {
//                Title = dto.Title.Trim(),

//                Content = dto.Content.Trim(),

//                // Aapke Post entity mein AuthorId hai
//                AuthorId = userId,

      

//                CreatedAt = DateTime.UtcNow,

//                IsPublished = false,

//                // Simple slug
//                Slug = dto.Title
//                    .Trim()
//                    .ToLower()
//                    .Replace(" ", "-")
//            };

//            var createdPost = await _postRepository.AddAsync(post);

//            return MapToDto(createdPost);
//        }


//        // ============================
//        // UPDATE POST
//        // ============================
//        public async Task<bool> UpdateAsync(
//            int id,
//            UpdatePostDto dto,
//            int userId)
//        {
//            // ID check
//            if (id <= 0)
//            {
//                return false;
//            }

//            // User ID check
//            if (userId <= 0)
//            {
//                return false;
//            }

//            // DTO check
//            if (dto == null)
//            {
//                return false;
//            }

//            // Title check
//            if (string.IsNullOrWhiteSpace(dto.Title))
//            {
//                return false;
//            }

//            // Content check
//            if (string.IsNullOrWhiteSpace(dto.Content))
//            {
//                return false;
//            }

//            var post = await _postRepository.GetByIdAsync(id);

//            // Post exist nahi karta
//            if (post == null)
//            {
//                return false;
//            }

//            // IMPORTANT:
//            // Sirf jis user ka post hai wahi update kar sakta hai
//            if (post.AuthorId != userId)
//            {
//                return false;
//            }

//            post.Title = dto.Title.Trim();

//            post.Content = dto.Content.Trim();

//            post.UpdatedAt = DateTime.UtcNow;

//            await _postRepository.UpdateAsync(post);

//            return true;
//        }


//        // ============================
//        // DELETE POST
//        // ============================
//        public async Task<bool> DeleteAsync(
//            int id,
//            int userId)
//        {
//            // ID check
//            if (id <= 0)
//            {
//                return false;
//            }

//            // User ID check
//            if (userId <= 0)
//            {
//                return false;
//            }

//            var post = await _postRepository.GetByIdAsync(id);

//            // Post nahi mila
//            if (post == null)
//            {
//                return false;
//            }

//            // IMPORTANT:
//            // Sirf owner apna post delete kar sakta hai
//            if (post.AuthorId != userId)
//            {
//                return false;
//            }

//            await _postRepository.DeleteAsync(id);

//            return true;
//        }


//        // ============================
//        // ENTITY -> DTO
//        // ============================
//        private static PostDto MapToDto(Post post)
//        {
//            return new PostDto
//            {
//                Id = post.Id,

//                Title = post.Title,

//                Content = post.Content,

//                UserId = post.AuthorId,

//                CreatedAt = post.CreatedAt,

//                UpdatedAt = post.UpdatedAt
//            };
//        }
//    }
//}