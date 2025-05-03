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
   // [Authorize]
    [ApiController]
    [Route("post/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetAllPosts();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _postService.GetPostById(id);
            if (post == null) return NotFound();
            return Ok(post);
        }

        [HttpPost("CreatePost/{userId}")]
        public async Task<IActionResult> Create([FromForm] CreatePostDto dto, string userId)
        {
            var post = await _postService.CreatePost(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
        }

        [HttpPut("UpdatePost/{id}/{userId}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdatePostDto dto, string userId)
        {
            var post = await _postService.GetPostById(id);
            if (post == null)
                return NotFound("Post not found");

            if (post.DoctorId != userId)
                return StatusCode(403, "You are not authorized to update this post");

            var updated = await _postService.UpdatePost(id, dto, userId);
            return Ok(updated);
        }




        [HttpDelete("DeletePost/{id}/{userId}")]
        public async Task<IActionResult> Delete(int id, string userId)
        {
            var post = await _postService.GetPostById(id);
            if (post == null)
                return NotFound("Post not found");

            if (post.DoctorId != userId)
                return StatusCode(403,"You are not authorized to delete this post");

            var deleted = await _postService.DeletePost(id, userId);
            return Ok("Post Deleted");
        }


    }




}
