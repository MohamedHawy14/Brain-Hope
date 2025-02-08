using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.Authentication.SingUp
{
    public class ResetPassword
    {
        [Required]
        public string Password { get; set; } = null!;

        [Compare("Password", ErrorMessage = "The Password and confirmation password do not match.")]
        public string ConfirmationPassword { get; set; }=null!;

        public string Email { get; set; }=null!;
        public string Token { get; set; }=null!;
    }
}
