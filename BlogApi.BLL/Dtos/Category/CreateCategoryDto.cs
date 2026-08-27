using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApi.BLL.DTOs.Category
{
    public class CreateCategoryDto
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}