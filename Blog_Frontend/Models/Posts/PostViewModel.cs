namespace BlogMVC.Models.Posts
{
    public class PostViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public bool IsPublished { get; set; }

        public int UserId { get; set; }

        public int CategoryId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public PostAuthorViewModel? Author { get; set; }

        public PostCategoryViewModel? Category { get; set; }
    }

    public class PostAuthorViewModel
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;
    }

    public class PostCategoryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}