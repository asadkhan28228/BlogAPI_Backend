using System.ComponentModel.DataAnnotations;

namespace BlogMVC.Models.Comments
{
    public class CreateCommentViewModel
    {
        public int PostId { get; set; }

        [Required]
        [StringLength(
            1000,
            MinimumLength = 1)]
        public string Content { get; set; } = string.Empty;
    }
}