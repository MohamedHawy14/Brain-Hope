using BrainHope.DataAcess.Contexts;
using BrainHope.DataAcess.Models.Posts;
using BrainHope.DataAcess.Repositry.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Repositry
{
    public class PostRepository : IPostRepository
    {
        private readonly BrainHopeDbContext _context;

        public PostRepository(BrainHopeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await _context.Posts.Include(p => p.Likes)
                                       .Include(p => p.Comments)
                                       .ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(int postId)
        {
            return await _context.Posts.Include(p => p.Likes)
                                       .Include(p => p.Comments)
                                       .FirstOrDefaultAsync(p => p.Id == postId);
        }

        public async Task AddPostAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
        }

        public void UpdatePost(Post post)
        {
            _context.Posts.Update(post);
        }

        public async Task DeletePost(Post post)
        {
            _context.Posts.Remove(post);
            await Task.CompletedTask; // If needed, replace with actual async DB operation
        }

        public async Task RemoveComment(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddLike(int postId, string userId)
        {
            var like = new PostLike { PostId = postId, UserId = userId };
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> GetLikeAsync(int postId, string userId)
        {
            return await _context.Likes.AnyAsync(like => like.PostId == postId && like.UserId == userId);
        }


        public async Task RemoveLike(int postId, string userId)
        {
            var like = await _context.Likes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddComment(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateComment(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }


        public async Task<Comment> GetCommentByIdAsync(int commentId)
        {
            return await _context.Comments.FindAsync(commentId);
        }

    }


}
