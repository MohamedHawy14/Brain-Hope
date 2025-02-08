using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.Authentication.SignIn
{
    public class SignInDTO
    {
        [Required(ErrorMessage ="Email Is Required!")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password Is Required!")]
        public string Password { get; set; }
    }
}
