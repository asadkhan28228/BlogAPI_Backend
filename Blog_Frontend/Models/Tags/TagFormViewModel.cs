using System.ComponentModel.DataAnnotations;

namespace BlogMVC.Models.Tags
{
    public class TagFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}