using BrainHope.DataAcess.Models.Posts;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.Services.DTO.Posts;
using BrainHope.Services.Hubs;
using BrainHope.Services.InterFaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilites;

namespace BrainHope.Services.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<PostHub> _postHub;

        public PostService(IPostRepository postRepository, IUnitOfWork unitOfWork, IHubContext<PostHub> postHub)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _postHub = postHub;
        }

        public async Task<IEnumerable<PostDto>> GetAllPosts()
        {
            var posts = await _postRepository.GetAllPostsAsync();
            return posts.Select(post => new PostDto
            {
                Id = post.Id,
                DoctorId = post.DoctorId,
                Title=post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                LikesCount = post.Likes.Count,
                CommentsCount = post.Comments.Count
            });
        }

        public async Task<PostDto> GetPostById(int id)
        {
            var post = await _postRepository.GetPostByIdAsync(id);
            return post == null ? null : new PostDto
            {
                Id = post.Id,
                Title=post.Title,
                DoctorId = post.DoctorId,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                LikesCount = post.Likes.Count,
                CommentsCount = post.Comments.Count
            };
        }

        public async Task<PostDto?> CreatePost(CreatePostDto dto, string doctorId)
        {
            string? imageUrl = null;

            if (dto.ImageUrl != null)
            {
                try
                {
                    imageUrl = await ImageHelper.SaveImageAsync(dto.ImageUrl);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Image upload failed: {ex.Message}");
                }
            }

            var post = new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                ImageUrl = imageUrl,
                DoctorId = doctorId,
                CreatedAt = DateTime.UtcNow
            };

            await _postRepository.AddPostAsync(post);
            await _unitOfWork.Complete();

            await _postHub.Clients.All.SendAsync("ReceivePostUpdate", post.Id);

            // Convert the created post to PostDto
            return new PostDto
            {
                Id = post.Id,
                DoctorId = post.DoctorId,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                LikesCount = 0, // Default to 0 since it's a new post
                CommentsCount = 0 // Default to 0 since it's a new post
            };
        }


        public async Task<PostDto?> UpdatePost(int id, UpdatePostDto dto, string doctorId)
        {
            var post = await _postRepository.GetPostByIdAsync(id);
            if (post == null || post.DoctorId != doctorId)
            {
                return null; // Post not found or unauthorized
            }

            // Keep old values if new ones are not provided
            post.Title = string.IsNullOrWhiteSpace(dto.Title) ? post.Title : dto.Title;
            post.Content = string.IsNullOrWhiteSpace(dto.Content) ? post.Content : dto.Content;

            if (dto.ImageUrl != null) // If a new image is uploaded, update it
            {
                try
                {
                    post.ImageUrl = await ImageHelper.SaveImageAsync(dto.ImageUrl);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Image upload failed: {ex.Message}");
                }
            }

            await _unitOfWork.Complete();

            return new PostDto
            {
                Id = post.Id,
                DoctorId = post.DoctorId,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                LikesCount = post.Likes.Count(),
                CommentsCount = post.Comments.Count()
            };
        }

        public async Task<bool> DeletePost(int id, string doctorId)
        {
            var post = await _postRepository.GetPostByIdAsync(id);
            if (post == null || post.DoctorId != doctorId) return false;

            await _postRepository.DeletePost(post); 
            await _unitOfWork.Complete();
            return true;
        }

        public async Task<bool> LikePost(int postId, string userId)
        {
            var post = await _postRepository.GetPostByIdAsync(postId);
            if (post == null) return false;

            // Check if the like already exists
            var existingLike = await _postRepository.GetLikeAsync(postId, userId);
            if (existingLike) return false; // Prevent duplicate likes

            await _postRepository.AddLike(postId, userId);
            await _unitOfWork.Complete();
            await _postHub.Clients.All.SendAsync("ReceiveLikeUpdate", postId, userId);
            return true;
        }

        public async Task<bool> UnlikePost(int postId, string userId)
        {
            // Ensure the like exists before removing
            var likeExists = await _postRepository.GetLikeAsync(postId, userId);
            if (!likeExists) return false; // If the like doesn't exist, no need to remove

            await _postRepository.RemoveLike(postId, userId);
            await _unitOfWork.Complete();
            await _postHub.Clients.All.SendAsync("ReceiveUnlikeUpdate", postId, userId);
            return true;
        }


        public async Task AddComment(CommentDto dto, string userId)
        {
            var comment = new Comment
            {
                PostId = dto.PostId, 
                UserId = userId,
                Content = dto.Content
            };

            await _postRepository.AddComment(comment);
            await _unitOfWork.Complete();

            await _postHub.Clients.All.SendAsync("ReceiveCommentUpdate", comment.PostId, comment.Id, comment.Content);
        }

        public async Task<bool> UpdateComment(int commentId, string userId, string content)
        {
            var comment = await _postRepository.GetCommentByIdAsync(commentId);
            if (comment == null || comment.UserId != userId) return false; // Ensure user owns the comment

            comment.Content = content;
            await _postRepository.UpdateComment(comment);
            await _unitOfWork.Complete();
            return true;
        }



        public async Task<bool> DeleteComment(int commentId, string userId, bool isAdmin)
        {
            var comment = await _postRepository.GetCommentByIdAsync(commentId);
            if (comment == null || (comment.UserId != userId && !isAdmin)) return false;

            await _postRepository.RemoveComment(comment.Id);  
            await _unitOfWork.Complete();
            return true;
        }

    }

}
