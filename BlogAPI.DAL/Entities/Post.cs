using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApi.DAL.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Slug { get; set; }              // URL-friendly title
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Foreign keys
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }

        // Navigation properties
        public User Author { get; set; }
        public Category Category { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<PostTag> PostTags { get; set; }
    }
}
