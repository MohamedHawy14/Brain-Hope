using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.Admin
{
    public class BlockUserDTO
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public bool BlockStatus { get; set; } // True means block, False means unblock
    }

}
