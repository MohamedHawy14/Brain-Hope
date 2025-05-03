using BrainHope.DataAcess.Models;
using BrainHope.Services.DTO.Posts;
using BrainHope.Services.InterFaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BrainHope_.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LikesController : ControllerBase
    {
        private readonly IPostService _postService;

        public LikesController(IPostService postService)
        {
            _postService = postService;
        }
        [HttpPost("like")]
        public async Task<IActionResult> LikePost([FromForm] PostLikeDto dto)
        {
            var postExists = await _postService.PostExists(dto.PostId);
            if (!postExists)
                return NotFound("Post not found");

            var result = await _postService.LikePost(dto);
            return result ? Ok("Post Liked") : BadRequest("Already liked");
        }

        [HttpPost("unlike")]
        public async Task<IActionResult> UnlikePost([FromForm] PostLikeDto dto)
        {
            var postExists = await _postService.PostExists(dto.PostId);
            if (!postExists)
                return NotFound("Post not found");

            var result = await _postService.UnlikePost(dto);
            return result ? Ok("Post UnLiked") : NotFound("Like not found");
        }

    }

}
