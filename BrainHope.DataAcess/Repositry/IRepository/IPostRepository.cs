using BrainHope.DataAcess.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Repositry.IRepository
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetAllPostsAsync();
        Task<Post> GetPostByIdAsync(int postId);
        Task AddPostAsync(Post post);
        void UpdatePost(Post post);
        Task DeletePost(Post post);

        Task AddComment(Comment comment);

        Task UpdateComment(Comment comment);
        Task RemoveComment(int commentId);
        Task AddLike(int postId, string userId);
        Task<bool> GetLikeAsync(int postId, string userId);
        Task RemoveLike(int postId, string userId);
        Task<Comment> GetCommentByIdAsync(int commentId);
    }

}
