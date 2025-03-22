using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Models.Posts
{
    public class Post
    {
        public int Id { get; set; }
        public string DoctorId { get; set; } // Only doctors can create posts
        public string Title { get; set; }
        public string Content { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }

        public virtual ApplicationUser Doctor { get; set; }
        public virtual ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }


}
