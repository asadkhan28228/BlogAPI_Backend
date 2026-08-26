using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApi.BLL.DTOs.Post
{
    public class PostDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}