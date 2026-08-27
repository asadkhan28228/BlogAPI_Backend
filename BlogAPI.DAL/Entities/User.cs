using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApi.DAL.Entities
{
    public class User
    {
        public int User_id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
       
        public string PasswordHash { get; set; }
        public string Role { get; set; } = "User";   // "User" or "Admin"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
