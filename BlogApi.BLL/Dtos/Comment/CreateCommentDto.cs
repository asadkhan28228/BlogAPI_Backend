using System.ComponentModel.DataAnnotations;

namespace BlogApi.BLL.DTOs.Comment
{
    public class CreateCommentDto
    {
        [Required(ErrorMessage = "Comment content is required.")]
        [StringLength(1000, MinimumLength = 1,
            ErrorMessage = "Comment must be between 1 and 1000 characters.")]
        public string Content { get; set; } = string.Empty;

        [Range(1, int.MaxValue,
            ErrorMessage = "Valid post ID is required.")]
        public int PostId { get; set; }
    }
}