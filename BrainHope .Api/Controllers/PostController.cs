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

namespace BrainHope_.Api.Controllers
{
    [Route("post/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<PostHub> _hubContext;

        public PostController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHubContext<PostHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [HttpGet("GetAllPosts")]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _unitOfWork.PostRepository.GetAllPostsAsync();
            var response = posts.Select(post => new
            {
                post.Id,
                post.DoctorId,
                post.Title,
                post.Content,
                ImageUrl = string.IsNullOrEmpty(post.ImageUrl) ? null : post.ImageUrl,
                post.CreatedAt
            });
            return Ok(response);
        }

        [HttpGet("GetPost/{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(id);
            if (post == null) return NotFound();

            var response = new
            {
                post.Id,
                post.DoctorId,
                post.Title,
                post.Content,
                ImageUrl = string.IsNullOrEmpty(post.ImageUrl) ? null : post.ImageUrl,
                post.CreatedAt
            };

            return Ok(response);
        }


        [HttpPost("Create")]
        [Authorize/*(Roles = SD.Role_Doctor)*/]
        public async Task<IActionResult> CreatePost([FromForm] PostDTO postDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var doctor = await _userManager.FindByIdAsync(userId);
            if (doctor == null)
            {
                return Unauthorized("Doctor not found in database.");
            }

            string imageUrl = null;
            if (postDto.ImageUrl != null)
            {
                imageUrl = await ImageHelper.SaveImageAsync(postDto.ImageUrl);
            }

            var post = new Post
            {
                DoctorId = doctor.Id,
                Title = postDto.Title,
                Content = postDto.Content,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow
            };

            var createdPost = await _unitOfWork.PostRepository.CreatePostAsync(post);
            await _unitOfWork.Complete();

            // Map the created post to PostResponseDto
            var postResponseDto = new PostResponseDto
            {
                Id = createdPost.Id,
                DoctorId = createdPost.DoctorId,
                DoctorName = doctor.UserName, // Assuming UserName is the doctor's name
                Title = createdPost.Title,
                Content = createdPost.Content,
                ImageUrl = createdPost.ImageUrl,
                CreatedAt = createdPost.CreatedAt
            };

            // Send Real-Time Notification
            await _hubContext.Clients.All.SendAsync("ReceivePostUpdate");

            return CreatedAtAction(nameof(GetPostById), new { id = createdPost.Id }, postResponseDto);
        }


        [HttpPut("Update/{id}")]
        [Authorize/*(Roles = SD.Role_Doctor)*/]
        public async Task<IActionResult> UpdatePost(int id, [FromForm] PostDTO updatedPostDto)
        {
            var existingPost = await _unitOfWork.PostRepository.GetPostByIdAsync(id);
            if (existingPost == null) return NotFound();

            var doctor = await _userManager.GetUserAsync(User);
            if (doctor == null || doctor.Id != existingPost.DoctorId) return Unauthorized();

            // Keep old image if new one is not provided
            string imageUrl = existingPost.ImageUrl;
            if (updatedPostDto.ImageUrl != null)
            {
                imageUrl = await ImageHelper.SaveImageAsync(updatedPostDto.ImageUrl);
            }

            // Keep old content & title if not provided
            existingPost.Title = string.IsNullOrWhiteSpace(updatedPostDto.Title)
                ? existingPost.Title
                : updatedPostDto.Title;

            existingPost.Content = string.IsNullOrWhiteSpace(updatedPostDto.Content)
                ? existingPost.Content
                : updatedPostDto.Content;

            existingPost.ImageUrl = imageUrl;
            existingPost.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.PostRepository.UpdatePostAsync(existingPost);
            await _unitOfWork.Complete();

            // Map to PostResponseDto
            var postResponseDto = new PostResponseDto
            {
                Id = existingPost.Id,
                DoctorId = existingPost.DoctorId,
                DoctorName = doctor.UserName, // Assuming UserName stores the doctor's name
                Title = existingPost.Title,
                Content = existingPost.Content,
                ImageUrl = existingPost.ImageUrl,
                CreatedAt = existingPost.CreatedAt
            };

            // Send real-time update
            await _hubContext.Clients.All.SendAsync("ReceivePostUpdate");

            return Ok(postResponseDto);
        }


        // ✅ Delete Post (Only Doctor)
        [HttpDelete("Delete/{id}")]
        [Authorize/*(Roles = SD.Role_Doctor)*/]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(id);
            if (post == null) return NotFound();

            var doctor = await _userManager.GetUserAsync(User);
            if (doctor == null || doctor.Id != post.DoctorId) return Unauthorized();

            var result = await _unitOfWork.PostRepository.DeletePostAsync(id);
            if (!result) return BadRequest("Failed to delete the post.");

            await _unitOfWork.Complete();

       
            await _hubContext.Clients.All.SendAsync("ReceivePostUpdate");

            return NoContent();
        }
    }


}
