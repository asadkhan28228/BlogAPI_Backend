namespace BlogMVC.Models.Posts
{
    public class PostPagedResultViewModel
    {
        public IEnumerable<PostViewModel> Items { get; set; }
            = new List<PostViewModel>();

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }
    }
}