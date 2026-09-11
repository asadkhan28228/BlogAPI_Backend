using System.ComponentModel.DataAnnotations;

namespace BlogMVC.Models.Posts
{
    public class UpdatePostViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(
            200,
            MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(10)]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        public bool IsPublished { get; set; }
    }
}