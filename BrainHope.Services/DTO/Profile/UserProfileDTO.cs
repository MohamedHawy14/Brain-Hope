using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.Profile
{
    public class UserProfileDTO 
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ProfilePhoto { get; set; }
        public string? Bio { get; set; } //Description in app user
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
