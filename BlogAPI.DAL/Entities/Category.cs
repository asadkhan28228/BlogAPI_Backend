using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApi.DAL.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation property
        public ICollection<Post> Posts { get; set; }
    }
}
