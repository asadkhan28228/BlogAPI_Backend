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

        public string Slug { get; set; }

        public bool IsPublished { get; set; }

        public int UserId { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreatedAt { get; set; }

       
    }
}