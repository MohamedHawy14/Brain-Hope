using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? Address { get; set; }

        [Required(ErrorMessage = "National ID is required.")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "National ID must be exactly 14 characters.")]
        public string NationalId { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
