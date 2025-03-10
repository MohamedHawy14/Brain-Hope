using BrainHope.DataAcess.Models;
using BrainHope.Services.DTO;
using BrainHope.Services.DTO.Authentication.SignIn;
using BrainHope.Services.DTO.Authentication.SingUp;
using BrainHope.Services.DTO.Authentication.User;
using BrainHope.Services.DTO.Email;
using BrainHope.Services.InterFaces;
using BrainHope.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BrainHope_.Api.Controllers
{
    [Route("Account/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthServices _authServices;

        public AccountController(UserManager<ApplicationUser> userManager ,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService,
            IConfiguration configuration,
            SignInManager<ApplicationUser> signInManager,
            IAuthServices authServices)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._emailService = emailService;
            this._configuration = configuration;
            this._signInManager = signInManager;
            this._authServices = authServices;
        }



        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterUser registerUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authServices.CreateUserWithTokenAsync(registerUser);

            if (!response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response);
            }

            // Generate email confirmation link
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account",
                new { token = response.Response.Token, email = registerUser.Email }, Request.Scheme);

            var message = new Message(new string[] { registerUser.Email! }, "Confirmation Email Link", confirmationLink!);
            _emailService.SendEmail(message);

            return Ok(response);
        }


        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK,
                   new Response { Status = "Success", Message = "Email Verified Successfully." , IsSuccess = true });
                }

            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                  new Response { Status = "Error", Message = "This Use Don't Exist." });

        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromForm] SignInDTO signInDTO)
        {
            
            var user = await _userManager.FindByEmailAsync(signInDTO.Email);
            if (user == null)
            {
                return Unauthorized(new Response { IsSuccess = false, Message = "User not found." , Status="Error"});
            }

            // Check if the email is confirmed.
            if (!user.EmailConfirmed)
            {
                return Unauthorized(new Response { IsSuccess = false, Message = "Please confirm your email to login.", Status = "Error" });
            }

            
            var passwordValid = await _userManager.CheckPasswordAsync(user, signInDTO.Password);
            if (!passwordValid)
            {
                return Unauthorized(new Response { IsSuccess = false, Message = "Invalid credentials.", Status = "Error" });
            }

           
            var tokenResponse = await _authServices.GetJwtTokenAsync(user);
            if (!tokenResponse.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, tokenResponse);
            }

            return Ok(tokenResponse);
        }


        [HttpPost("ForgetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword([FromForm] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new Response { IsSuccess = false, Message = "User not found.", Status = "Error" });
            }

            var otp = GenerateSimpleOtp(user.Id);
            var message = new Message(new string[] { user.Email! }, "Password Reset OTP", $"Your OTP is: {otp}");
            _emailService.SendEmail(message);

            HttpContext.Session.SetString("ResetPasswordUserId", user.Id);
            HttpContext.Session.SetString("ResetPasswordOtp", otp); // Store the OTP in session

            return Ok(new Response { IsSuccess = true, Message = $"OTP sent to {user.Email}.", Status = "Success" });
        }


        [HttpPost("VerifyOtp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromForm] string otp)
        {
            var userId = HttpContext.Session.GetString("ResetPasswordUserId");
            var storedOtp = HttpContext.Session.GetString("ResetPasswordOtp");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(storedOtp))
            {
                return BadRequest(new Response { IsSuccess = false, Message = "User session expired.", Status = "Error" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new Response { IsSuccess = false, Message = "User not found.", Status = "Error" });
            }

            if (storedOtp != otp)
            {
                return BadRequest(new Response { IsSuccess = false, Message = "Invalid OTP.", Status = "Error" });
            }

            HttpContext.Session.SetString("ResetPasswordUserId", user.Id);

            return Ok(new Response { IsSuccess = true, Message = "OTP verified successfully.", Status = "Success" });
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPassword resetPasswordRequest)
        {
            // Retrieve the UserId from the session
            var userId = HttpContext.Session.GetString("ResetPasswordUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new Response { IsSuccess = false, Message = "User session expired.", Status = "Error" });
            }

            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new Response { IsSuccess = false, Message = "User not found.", Status = "Error" });
            }

           
            if (resetPasswordRequest.NewPassword != resetPasswordRequest.ConfirmNewPassword)
            {
                return BadRequest(new Response { IsSuccess = false, Message = "Passwords do not match.", Status = "Error" });
            }

           
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            
            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordRequest.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Password reset failed.",
                    Status = "Error"
                   
                });
            }

            // Clear the session after successful password reset
            HttpContext.Session.Remove("ResetPasswordUserId");

            return Ok(new Response
            {
                IsSuccess = true,
                Message = "Password reset successfully.",
                Status = "Success"
            });
        }

     



        #region Private Methods
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigninkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secert"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(30),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninkey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }

        private string GenerateSimpleOtp()
        {
            var random = new Random();
            return random.Next(1000, 9999).ToString(); 
        }
        private string GenerateSimpleOtp(string userId)
        {
            var secretKey = _configuration["OtpSecretKey"];
            var currentTime = DateTime.UtcNow.ToString("yyyyMMddHHmm");
            var data = $"{userId}{secretKey}{currentTime}";

            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                var otp = (BitConverter.ToInt32(hash, 0) % 10000);
                return Math.Abs(otp).ToString("D4");
            }
        }
        #endregion




    }
}
