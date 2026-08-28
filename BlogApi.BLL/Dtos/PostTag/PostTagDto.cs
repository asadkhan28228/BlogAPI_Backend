using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApi.BLL.DTOs.PostTag
{
    public class PostTagDto
    {
        public int PostId { get; set; }

        public int TagId { get; set; }

        public string TagName { get; set; }
    }
}