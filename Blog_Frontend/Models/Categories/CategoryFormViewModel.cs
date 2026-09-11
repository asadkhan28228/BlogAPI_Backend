using System.ComponentModel.DataAnnotations;

namespace BlogMVC.Models.Categories
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}