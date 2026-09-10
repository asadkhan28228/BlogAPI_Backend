using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApi.BLL.DTOs.Post
{
    public class PostCategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}