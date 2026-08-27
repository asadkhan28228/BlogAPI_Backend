//using BlogApi.BLL.DTOs.Post;
//using BlogApi.BLL.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace BlogApi.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class PostsController : ControllerBase
//    {
//        private readonly IPostService _postService;

//        public PostsController(IPostService postService)
//        {
//            _postService = postService;
//        }

//        // =========================
//        // GET ALL POSTS
//        // =========================
//        [HttpGet]
     
//        public async Task<IActionResult> GetAll()
//        {
//            var posts = await _postService.GetAllAsync();

//            if (posts == null || !posts.Any())
//            {
//                return NotFound("No posts found.");
//            }

//            return Ok(posts);
//        }


//        // =========================
//        // GET POST BY ID
//        // =========================
//        [HttpGet("{id}")]

//        public async Task<IActionResult> GetById(int id)
//        {
//            if (id <= 0)
//            {
//                return BadRequest("Invalid post ID.");
//            }

//            var post = await _postService.GetByIdAsync(id);

//            if (post == null)
//            {
//                return NotFound("Post not found.");
//            }

//            return Ok(post);
//        }

//        [HttpPost]
    
//        public async Task<IActionResult> Create(CreatePostDto dto)
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//            if (string.IsNullOrEmpty(userId))
//            {
//                return Unauthorized("User ID not found.");
//            }

//            if (!int.TryParse(userId, out int userIdValue))
//            {
//                return Unauthorized("Invalid User ID.");
//            }

//            var post = await _postService.CreateAsync(dto, userIdValue);

//            return CreatedAtAction(
//                nameof(GetById),
//                new { id = post.Id },
//                post
//            );
//        }


//        // =========================
//        // UPDATE POST
//        // =========================
//        [HttpPut("{id}")]
     
//        public async Task<IActionResult> Update(int id,UpdatePostDto dto)
//        {
//            if (id <= 0)
//            {
//                return BadRequest("Invalid post ID.");
//            }

//            if (dto == null)
//            {
//                return BadRequest("Post data is required.");
//            }

//            var userIdValue =
//                User.FindFirstValue(ClaimTypes.NameIdentifier);

//            if (string.IsNullOrEmpty(userIdValue))
//            {
//                return Unauthorized("User ID not found.");
//            }

//            if (!int.TryParse(userIdValue, out int userId))
//            {
//                return Unauthorized("Invalid user ID.");
//            }

//            var result = await _postService.UpdateAsync(id,dto,userId);

//            if (!result)
//            {
//                return NotFound(
//                    "Post not found or you are not the owner."
//                );
//            }

//            return Ok("Post updated successfully.");
//        }


//        // =========================
//        // DELETE POST
//        // =========================
//        [HttpDelete("{id}")]
    
//        public async Task<IActionResult> Delete(int id)
//        {
//            if (id <= 0)
//            {
//                return BadRequest("Invalid post ID.");
//            }

//            var userIdValue =
//                User.FindFirstValue(ClaimTypes.NameIdentifier);

//            if (string.IsNullOrEmpty(userIdValue))
//            {
//                return Unauthorized("User ID not found.");
//            }

//            if (!int.TryParse(userIdValue, out int userId))
//            {
//                return Unauthorized("Invalid user ID.");
//            }

//            var result = await _postService.DeleteAsync(
//                id,
//                userId
//            );

//            if (!result)
//            {
//                return NotFound(
//                    "Post not found or you are not the owner."
//                );
//            }

//            return Ok("Post deleted successfully.");
//        }
//    }
//}