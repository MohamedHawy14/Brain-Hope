using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.Posts
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string? UserPhoto { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
