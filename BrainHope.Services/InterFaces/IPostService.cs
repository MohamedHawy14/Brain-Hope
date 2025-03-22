using BrainHope.Services.DTO.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.InterFaces
{
    public interface IPostService
    {
        Task<IEnumerable<PostDto>> GetAllPosts();
        Task<PostDto> GetPostById(int id);
        Task<PostDto?> CreatePost(CreatePostDto dto, string doctorId);
        Task<PostDto?> UpdatePost(int id, UpdatePostDto dto, string doctorId);
        Task<bool> DeletePost(int id, string doctorId);

        Task<bool> LikePost(int postId, string userId);
        Task<bool> UnlikePost(int postId, string userId);

        Task AddComment(CommentDto dto, string userId);
        Task<bool> UpdateComment(int commentId, string userId, string content);
        Task<bool> DeleteComment(int commentId, string userId, bool isAdmin);
    }

}
