using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.Profile
{
    public class UpdateProfileDTO
    {
        public string? UserName { get; set; }


        public string? Bio { get; set; } // Maps to ApplicationUser.Description


        public string? Address { get; set; }


        public string? PhoneNumber { get; set; }
    }
}
