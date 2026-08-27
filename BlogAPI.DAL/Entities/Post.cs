using System;
using System.Collections.Generic;

namespace BlogApi.DAL.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Slug { get; set; }
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public int CategoryId { get; set; }


        // Navigation Properties
        public User User { get; set; }

        public Category Category { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}