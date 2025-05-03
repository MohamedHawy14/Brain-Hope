using BrainHope.DataAcess.Contexts;
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
        private readonly BrainHopeDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<PostHub> _postHub;

        public PostService(BrainHopeDbContext context, IUnitOfWork unitOfWork, IHubContext<PostHub> postHub)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _postHub = postHub;
        }

        public async Task<IEnumerable<PostDto>> GetAllPosts()
        {
            var posts = await _context.Posts
                .Include(p => p.Doctor)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .ToListAsync();

            return posts.Select(post => new PostDto
            {
                Id = post.Id,
                DoctorId = post.DoctorId,
                DoctorName = post.Doctor.UserName,
                DoctorPhoto = post.Doctor.ProfilePhoto,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                LikesCount = post.Likes.Count,
                CommentsCount = post.Comments.Count,
                Comments = post.Comments.Select(comment => new CommentDto
                {
                    Id = comment.Id,
                    PostId = comment.PostId,
                    UserId = comment.UserId,
                    UserName = comment.User.UserName,
                    UserPhoto = comment.User.ProfilePhoto,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt
                }).ToList()
            });
        }

        public async Task<PostDto> GetPostById(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Doctor)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return null;

            return new PostDto
            {
                Id = post.Id,
                DoctorId = post.DoctorId,
                DoctorName = post.Doctor.UserName,
                DoctorPhoto = post.Doctor.ProfilePhoto,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                LikesCount = post.Likes.Count,
                CommentsCount = post.Comments.Count,
                Comments = post.Comments.Select(comment => new CommentDto
                {
                    Id = comment.Id,
                    PostId = comment.PostId,
                    UserId = comment.UserId,
                    UserName = comment.User.UserName,
                    UserPhoto = comment.User.ProfilePhoto,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt
                }).ToList()
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

            _context.Posts.Add(post);
            await _unitOfWork.Complete();

            await _postHub.Clients.All.SendAsync("ReceivePostUpdate", post.Id);

            var doctor = await _context.Users.FindAsync(doctorId);

            return new PostDto
            {
                Id = post.Id,
                DoctorId = post.DoctorId,
                DoctorName = doctor?.UserName,
                DoctorPhoto = doctor?.ProfilePhoto,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                LikesCount = 0,
                CommentsCount = 0,
                Comments = new List<CommentDto>()
            };
        }

        public async Task<PostDto?> UpdatePost(int id, UpdatePostDto dto, string doctorId)
        {
            var post = await _context.Posts
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null || post.DoctorId != doctorId) return null;

            post.Title = string.IsNullOrWhiteSpace(dto.Title) ? post.Title : dto.Title;
            post.Content = string.IsNullOrWhiteSpace(dto.Content) ? post.Content : dto.Content;

            if (dto.ImageUrl != null)
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

            var doctor = await _context.Users.FindAsync(doctorId);

            return new PostDto
            {
                Id = post.Id,
                DoctorId = post.DoctorId,
                DoctorName = doctor?.UserName,
                DoctorPhoto = doctor?.ProfilePhoto,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                LikesCount = post.Likes.Count(),
                CommentsCount = post.Comments.Count(),
                Comments = post.Comments.Select(comment => new CommentDto
                {
                    Id = comment.Id,
                    PostId = comment.PostId,
                    UserId = comment.UserId,
                    UserName = comment.User.UserName,
                    UserPhoto = comment.User.ProfilePhoto,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt
                }).ToList()
            };
        }

        public async Task<bool> DeletePost(int id, string doctorId)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null || post.DoctorId != doctorId) return false;

            _context.Posts.Remove(post);
            await _unitOfWork.Complete();
            return true;
        }

        public async Task<bool> PostExists(int postId)
        {
            return await _context.Posts.AnyAsync(p => p.Id == postId);
        }

        public async Task<bool> LikePost(PostLikeDto dto)
        {
            var existingLike = await _context.Likes
                .AnyAsync(l => l.PostId == dto.PostId && l.UserId == dto.UserId);

            if (existingLike)
                return false;

            _context.Likes.Add(new PostLike { PostId = dto.PostId, UserId = dto.UserId });
            await _unitOfWork.Complete();

            await _postHub.Clients.All.SendAsync("ReceiveLikeUpdate", dto.PostId, dto.UserId);
            return true;
        }

        public async Task<bool> UnlikePost(PostLikeDto dto)
        {
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == dto.PostId && l.UserId == dto.UserId);

            if (like == null)
                return false;

            _context.Likes.Remove(like);
            await _unitOfWork.Complete();

            await _postHub.Clients.All.SendAsync("ReceiveUnlikeUpdate", dto.PostId, dto.UserId);
            return true;
        }

        public async Task<Comment> AddComment(CreateCommentDto dto, string userId)
        {
            var comment = new Comment
            {
                PostId = dto.PostId,
                UserId = userId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _unitOfWork.Complete();

            await _postHub.Clients.All.SendAsync("ReceiveCommentUpdate", comment.PostId, comment.Id, comment.Content);
            return comment;
        }

        public async Task<bool> UpdateComment(int commentId, string userId, UpdateCommentDto dto)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null || comment.UserId != userId) return false;

            comment.Content = dto.Content;
            await _unitOfWork.Complete();
            return true;
        }

        public async Task<bool> DeleteComment(int commentId, string userId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null || (comment.UserId != userId)) return false;

            _context.Comments.Remove(comment);
            await _unitOfWork.Complete();
            return true;
        }
    }

}
