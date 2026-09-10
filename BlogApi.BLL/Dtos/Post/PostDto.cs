namespace BlogApi.BLL.DTOs.Post
{
    public class PostDto
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

        // Author information
        public PostAuthorDto? Author { get; set; }

        // Category information
        public PostCategoryDto? Category { get; set; }
    }
}