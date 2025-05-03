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
    //[Authorize]
    [ApiController]
    [Route("comment/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(IPostService postService, UserManager<ApplicationUser> userManager)
        {
            _postService = postService;
            _userManager = userManager;
        }

        [HttpPost("Create/{userId}")]
        public async Task<IActionResult> AddComment(string userId, [FromForm] CreateCommentDto dto)
        {
            var comment = await _postService.AddComment(dto, userId);
            var user = await _userManager.FindByIdAsync(userId);

            var resultDto = new CommentDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                UserId = comment.UserId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UserName = user?.UserName,
                UserPhoto = user?.ProfilePhoto
            };

            return Ok(resultDto);
        }

        [HttpPut("Update/{commentId}/{userId}")]
        public async Task<IActionResult> UpdateComment(int commentId, string userId, [FromForm] UpdateCommentDto dto)
        {
            var result = await _postService.UpdateComment(commentId, userId, dto);

            if (!result)
                return NotFound("Comment not found or you are not authorized to update it.");

            return Ok("Comment updated successfully.");
        }

        [HttpDelete("Delete/{commentId}/{userId}")]
        public async Task<IActionResult> DeleteComment(int commentId, string userId)
        {
            var result = await _postService.DeleteComment(commentId, userId);

            if (!result)
                return NotFound("Comment not found or you are not authorized to delete it.");

            return Ok("Comment deleted successfully.");
        }

    }

}
