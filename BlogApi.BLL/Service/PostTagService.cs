using BlogApi.BLL.DTOs.PostTag;
using BlogApi.BLL.Interfaces;
using BlogApi.DAL.Entities;
using BlogApi.DAL.InterfacesRepositories;

namespace BlogApi.BLL.Services
{
    public class PostTagService : IPostTagService
    {
        private readonly IPostTagRepository _postTagRepository;

        private readonly IPostRepository _postRepository;

        private readonly ITagRepository _tagRepository;

        public PostTagService(
            IPostTagRepository postTagRepository,
            IPostRepository postRepository,
            ITagRepository tagRepository)
        {
            _postTagRepository = postTagRepository;

            _postRepository = postRepository;

            _tagRepository = tagRepository;
        }

        // =========================
        // GET TAGS OF POST
        // =========================
        public async Task<IEnumerable<PostTagDto>> GetByPostIdAsync(
            int postId)
        {
            if (postId <= 0)
            {
                return Enumerable.Empty<PostTagDto>();
            }

            var post =
                await _postRepository.GetByIdAsync(postId);

            if (post == null)
            {
                return Enumerable.Empty<PostTagDto>();
            }

            var postTags =
                await _postTagRepository
                    .GetByPostIdAsync(postId);

            return postTags.Select(pt => new PostTagDto
            {
                PostId = pt.PostId,

                TagId = pt.TagId,

                TagName = pt.Tag?.Name
            });
        }

        // =========================
        // ADD TAG TO POST
        // =========================
        public async Task<PostTagDto> AddTagToPostAsync(
            int postId,
            AddPostTagDto dto,
            int userId)
        {
            if (postId <= 0)
            {
                throw new ArgumentException(
                    "Invalid post ID.");
            }

            if (dto == null)
            {
                throw new ArgumentNullException(
                    nameof(dto));
            }

            if (dto.TagId <= 0)
            {
                throw new ArgumentException(
                    "Invalid tag ID.");
            }

            if (userId <= 0)
            {
                throw new ArgumentException(
                    "Invalid user ID.");
            }

            // Check Post
            var post =
                await _postRepository.GetByIdAsync(postId);

            if (post == null)
            {
                throw new ArgumentException(
                    "Post not found.");
            }

            // Only post owner can add tags
            if (post.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "You are not the owner of this post.");
            }

            // Check Tag
            var tag =
                await _tagRepository.GetByIdAsync(
                    dto.TagId);

            if (tag == null)
            {
                throw new ArgumentException(
                    "Tag not found.");
            }

            // Check duplicate
            var existing =
                await _postTagRepository.GetAsync(
                    postId,
                    dto.TagId);

            if (existing != null)
            {
                throw new ArgumentException(
                    "Tag is already assigned to this post.");
            }

            var postTag = new PostTag
            {
                PostId = postId,

                TagId = dto.TagId
            };

            var created =
                await _postTagRepository.AddAsync(
                    postTag);

            return new PostTagDto
            {
                PostId = created.PostId,

                TagId = created.TagId,

                TagName = tag.Name
            };
        }

        // =========================
        // REMOVE TAG FROM POST
        // =========================
        public async Task<bool> RemoveTagFromPostAsync(
            int postId,
            int tagId,
            int userId)
        {
            if (postId <= 0 ||
                tagId <= 0 ||
                userId <= 0)
            {
                return false;
            }

            // Check Post
            var post =
                await _postRepository.GetByIdAsync(postId);

            if (post == null)
            {
                return false;
            }

            // Only post owner can remove tag
            if (post.UserId != userId)
            {
                return false;
            }

            // Check PostTag
            var postTag =
                await _postTagRepository.GetAsync(
                    postId,
                    tagId);

            if (postTag == null)
            {
                return false;
            }

            await _postTagRepository.DeleteAsync(
                postId,
                tagId);

            return true;
        }
    }
}