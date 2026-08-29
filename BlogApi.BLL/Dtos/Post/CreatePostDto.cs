using System.ComponentModel.DataAnnotations;

namespace BlogApi.BLL.DTOs.Post
{
    public class CreatePostDto
    {
        [Required(ErrorMessage = "Post title is required.")]
        [StringLength(200, MinimumLength = 3,
            ErrorMessage = "Title must be between 3 and 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Post content is required.")]
        [MinLength(10,
            ErrorMessage = "Content must be at least 10 characters.")]
        public string Content { get; set; } = string.Empty;

        [Range(1, int.MaxValue,
            ErrorMessage = "Valid category ID is required.")]
        public int CategoryId { get; set; }

        public bool IsPublished { get; set; }
    }
}