using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApi.BLL.DTOs.Post
{
    public class PostAuthorDto
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;
    }
}