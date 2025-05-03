using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.Posts
{
    public class CreateCommentDto
    {
        public int PostId { get; set; }
        public string Content { get; set; }
    }

}
