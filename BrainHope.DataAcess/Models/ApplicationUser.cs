using BrainHope.DataAcess.Models.Chat;
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

        public string? Description { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

     

        // Profile Photo stored as byte array
        public byte[]? ProfilePhoto { get; set; }

        // Relation with Users
        public virtual Doctor? Doctor { get; set; }
        public virtual Patient? Patient { get; set; }
        public virtual Admin? Admin { get; set; }

        //chat
        public ICollection<UserConnection> UserConnections { get; set; }
    }
}
