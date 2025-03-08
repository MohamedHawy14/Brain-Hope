using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.Profile
{
    public class CompleteprofileDTO
    {
        [Required]
        public string Bio { get; set; } // Maps to ApplicationUser.Description

        [Required]
        public string Address { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
    }
}
