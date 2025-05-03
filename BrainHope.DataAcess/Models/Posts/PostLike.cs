using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Models.Posts
{
    public class PostLike:ModelBase
    {
      
        public int PostId { get; set; }
        public string UserId { get; set; }

        public virtual Post Post { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

}
