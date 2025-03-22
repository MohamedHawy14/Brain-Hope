using BrainHope.DataAcess.Models.Posts;
using BrainHope.DataAcess.Models;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.Services.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Utilites;
using BrainHope.Services.DTO.Posts;
using System.Security.Claims;
using BrainHope.Services.InterFaces;

namespace BrainHope_.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("post/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(IPostService postService, UserManager<ApplicationUser> userManager)
        {
            _postService = postService;
            _userManager = userManager;
        }


        [HttpGet("AllPosts")]
        public async Task<IActionResult> GetAllPosts()
        {
            return Ok(await _postService.GetAllPosts());
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _postService.GetPostById(id);
            if (post == null)
                return NotFound("Post not found.");

            return Ok(post);
        }

        [Authorize(Roles = SD.Role_Doctor)]
        [HttpPost("Create")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto dto)
        {
            var doctorId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(doctorId)) return Unauthorized("User not authenticated.");

            var postDto = await _postService.CreatePost(dto, doctorId);
            if (postDto == null) return BadRequest("Failed to create post.");

            return Ok(postDto);
        }



        [Authorize(Roles = SD.Role_Doctor)]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromForm] UpdatePostDto dto)
        {
            var doctorId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(doctorId)) return Unauthorized("User not authenticated.");

            var updatedPost = await _postService.UpdatePost(id, dto, doctorId);

            return updatedPost != null ? Ok(updatedPost) : NotFound("Post not found or you are not authorized to update it.");
        }



        [Authorize(Roles = SD.Role_Doctor)]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var doctorId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(doctorId)) return Unauthorized("User not authenticated.");
            var result = await _postService.DeletePost(id, doctorId);
            if (!result) return NotFound("Post not found or unauthorized.");

            return Ok("Post deleted successfully.");
        }

    }



}
