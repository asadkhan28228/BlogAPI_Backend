using BlogApi.BLL.Dtos.Comment;
using BlogApi.BLL.DTOs.Comment;
using BlogApi.BLL.Interfaces;
using BlogApi.DAL.Entities;
using BlogApi.DAL.InterfacesRepositories;

namespace BlogApi.BLL.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;

        public CommentService(
            ICommentRepository commentRepository,
            IPostRepository postRepository)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
        }

        // =========================
        // GET ALL COMMENTS
        // =========================
        public async Task<IEnumerable<CommentDto>> GetAllAsync()
        {
            var comments = await _commentRepository.GetAllAsync();

            return comments.Select(MapToDto);
        }

        // =========================
        // GET COMMENTS BY POST
        // =========================
        public async Task<IEnumerable<CommentDto>> GetByPostIdAsync(
            int postId)
        {
            if (postId <= 0)
            {
                return Enumerable.Empty<CommentDto>();
            }

            var post = await _postRepository.GetByIdAsync(postId);

            if (post == null)
            {
                return Enumerable.Empty<CommentDto>();
            }

            var comments =
                await _commentRepository.GetByPostIdAsync(postId);

            return comments.Select(MapToDto);
        }

        // =========================
        // GET COMMENT BY ID
        // =========================
        public async Task<CommentDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var comment =
                await _commentRepository.GetByIdAsync(id);

            if (comment == null)
            {
                return null;
            }

            return MapToDto(comment);
        }

        // =========================
        // CREATE COMMENT
        // =========================
        public async Task<CommentDto> CreateAsync(
            CreateCommentDto dto,
            int userId)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (userId <= 0)
            {
                throw new ArgumentException(
                    "Invalid user ID.");
            }

            if (dto.PostId <= 0)
            {
                throw new ArgumentException(
                    "Invalid post ID.");
            }

            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                throw new ArgumentException(
                    "Comment content is required.");
            }

            // Check Post
            var post =
                await _postRepository.GetByIdAsync(dto.PostId);

            if (post == null)
            {
                throw new ArgumentException(
                    "Post not found.");
            }

            var comment = new Comment
            {
                PostId = dto.PostId,

                UserId = userId,

                Content = dto.Content.Trim(),

                CreatedAt = DateTime.UtcNow

            };

            var createdComment =
                await _commentRepository.AddAsync(comment);

            return MapToDto(createdComment);
        }

        // =========================
        // UPDATE COMMENT
        // =========================
        public async Task<bool> UpdateAsync(int id,UpdateCommentDto dto,int userId)
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

            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return false;
            }

            var comment =
                await _commentRepository.GetByIdAsync(id);

            if (comment == null)
            {
                return false;
            }

            // Only comment owner can update
            if (comment.UserId != userId)
            {
                return false;
            }

            comment.Content = dto.Content.Trim();

            

            await _commentRepository.UpdateAsync(comment);

            return true;
        }

        // =========================
        // DELETE COMMENT
        // =========================
        public async Task<bool> DeleteAsync(
            int id,
            int userId)
        {
            if (id <= 0)
            {
                return false;
            }

            if (userId <= 0)
            {
                return false;
            }

            var comment =
                await _commentRepository.GetByIdAsync(id);

            if (comment == null)
            {
                return false;
            }

            // Only comment owner can delete
            if (comment.UserId != userId)
            {
                return false;
            }

            await _commentRepository.DeleteAsync(id);

            return true;
        }

        // =========================
        // MAP ENTITY TO DTO
        // =========================
        private static CommentDto MapToDto(
            Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,

                PostId = comment.PostId,

                UserId = comment.UserId,

                Content = comment.Content,

                CreatedAt = comment.CreatedAt

             
            };
        }
    }
}