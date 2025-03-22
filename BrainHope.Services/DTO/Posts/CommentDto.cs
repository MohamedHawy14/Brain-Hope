using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.Posts
{
    public class CommentDto
    {
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
    }

}
