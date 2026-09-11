using System.ComponentModel.DataAnnotations;

namespace BlogMVC.Models.Users
{
    public class UpdateRoleViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}