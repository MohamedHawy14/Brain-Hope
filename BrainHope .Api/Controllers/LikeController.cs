using BrainHope.DataAcess.Models;
using BrainHope.Services.InterFaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BrainHope_.Api.Controllers
{
    [Route("like/[controller]")]
    [ApiController]
    [Authorize]
    public class LikeController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly UserManager<ApplicationUser> _userManager;

        public LikeController(IPostService postService, UserManager<ApplicationUser> userManager)
        {
            _postService = postService;
            _userManager = userManager;
        }

        
        [HttpPost("{postId}")]
        public async Task<IActionResult> LikePost(int postId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized("User not authenticated.");

            var result = await _postService.LikePost(postId, userId);
            return result ? Ok("Post liked.") : BadRequest("Failed to like post.");
        }

       
        [HttpDelete("{postId}")]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized("User not authenticated.");

            var result = await _postService.UnlikePost(postId, userId);
            return result ? Ok("Post unliked.") : BadRequest("Failed to unlike post.");
        }
    }
}
