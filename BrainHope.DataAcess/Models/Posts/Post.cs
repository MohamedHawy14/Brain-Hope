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
        public string DoctorId { get; set; } // FK to AspNetUsers
        public string Title { get; set; }
        public string Content { get; set; }
        public string? ImageUrl { get; set; } // Optional image
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("DoctorId")]
        public virtual ApplicationUser Doctor { get; set; } // Relationship with Users
    }

}
