using BrainHope.DataAcess.Models;
using BrainHope.Services.DTO.Posts;
using BrainHope.Services.InterFaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Utilites;

namespace BrainHope_.Api.Controllers
{
    [Route("comment/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(IPostService postService, UserManager<ApplicationUser> userManager)
        {
            _postService = postService;
            _userManager = userManager;
        }

       
        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromForm] CommentDto dto)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized("User not authenticated.");

            await _postService.AddComment(dto, userId);
            return Ok("Comment added.");
        }


        [HttpPut("comments/{commentId}")]
        public async Task<IActionResult> UpdateComment(int commentId, [FromBody] UpdateCommentDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var result = await _postService.UpdateComment(commentId, userId, dto.Content);
            if (!result) return BadRequest("Failed to update comment or unauthorized.");

            return Ok("Comment updated successfully.");
        }



        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized("User not authenticated.");
            var isAdmin = User.IsInRole(SD.Role_Admin);

            var result = await _postService.DeleteComment(commentId, userId, isAdmin);
            return result ? Ok("Comment deleted.") : NotFound("Comment not found or unauthorized.");
        }
    }
}
