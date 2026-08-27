using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApi.BLL.Dtos.Comment
{
    public class CommentDto
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public int UserId { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
