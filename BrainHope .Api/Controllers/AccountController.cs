using BrainHope.DataAcess.Models;
using BrainHope.Services.DTO;
using BrainHope.Services.DTO.Authentication.SignIn;
using BrainHope.Services.DTO.Authentication.SingUp;
using BrainHope.Services.DTO.Email;
using BrainHope.Services.InterFaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
            var tokenResponse = await _authServices.CreateUserWithTokenAsync(registerUser);

            // If user creation failed, return error response
            if (!tokenResponse.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Message = tokenResponse.Message, IsSuccess = false });
            }

            // Assign roles to user
            await _authServices.AssignRoleToUserAsync(registerUser.Roles, tokenResponse.Response.User);

            // Generate email confirmation link
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account",
                new { token = tokenResponse.Response.Token, email = registerUser.Email }, Request.Scheme);

            var message = new Message(new string[] { registerUser.Email! }, "Confirmation Email Link", confirmationLink!);
            _emailService.SendEmail(message);

            return StatusCode(StatusCodes.Status200OK,
                new Response { Status = "Success", Message = $"User Created Successfully & Email Sent To {registerUser.Email} Successfully.", IsSuccess = true });
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
        public async Task<IActionResult> LogIn([FromBody] SignInDTO signInDTO)
        {

            var loginOtpResponse = await _authServices.GetOtpByLoginAsync(signInDTO);
            if (loginOtpResponse.Response != null) 
            {
                var user = loginOtpResponse.Response.User;
                if (user.TwoFactorEnabled)
                {

                    var token = loginOtpResponse.Response.Token;

                    var message = new Message(new string[] { user.Email! }, "OTP Confrimation", token);
                    _emailService.SendEmail(message);

                    return StatusCode(StatusCodes.Status200OK,
                     new Response {IsSuccess=loginOtpResponse.IsSuccess, Status = "Success", Message = $"We have sent an OTP to your Email {user.Email}" });
                }
                if (user != null && await _userManager.CheckPasswordAsync(user, signInDTO.Password))
                {
                    var authclaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),

                };
                    var userRoles = await _userManager.GetRolesAsync(user);
                    foreach (var Role in userRoles)
                    {
                        authclaims.Add(new Claim(ClaimTypes.Role, Role));
                    }

                    var jwtToken = GetToken(authclaims);
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        expiration = jwtToken.ValidTo
                    });


                }
            }
            
            return Unauthorized();



        }

        [HttpPost]
        [Route("login-2FA")]
        public async Task<IActionResult> LoginWithOTP(string code, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);
            if (signIn.Succeeded)
            {
                if (user != null)
                {
                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                    var userRoles = await _userManager.GetRolesAsync(user);
                    foreach (var role in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var jwtToken = GetToken(authClaims);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        expiration = jwtToken.ValidTo
                    });
                  

                }
            }
            return StatusCode(StatusCodes.Status404NotFound,
                new Response { Status = "Success", Message = $"Invalid Code" });
        }

        [HttpPost("ForgetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword([Required]string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var forgetPasswordlink = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, Request.Scheme);
                var message = new Message(new string[] { user.Email! }, "Forget Password Link", forgetPasswordlink!);
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK,
                       new Response { Status = "Success", Message = $"Password Changed Request is sent to {user.Email} Successfully. Please Open Ur Email And Click On Link" });
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                      new Response { Status = "Error", Message = "Couldn't Send link to email , please try again." });
        
        }

        [HttpGet("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string token , string email)
        {
            var model = new ResetPassword {Token=token , Email=email };
            return Ok(new { model });

        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user != null)
            {
                var resetPassresult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if (!resetPassresult.Succeeded)
                {
                   foreach(var error in resetPassresult.Errors)
                   {
                        ModelState.AddModelError(error.Code, error.Description);
                   }
                    return Ok(ModelState);
                }
                
                return StatusCode(StatusCodes.Status200OK,
                       new Response { Status = "Success", Message =$"Password Has Been Changed " });
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                      new Response { Status = "Error", Message = "SomeThing were Wrong." });

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
        #endregion




    }
}
