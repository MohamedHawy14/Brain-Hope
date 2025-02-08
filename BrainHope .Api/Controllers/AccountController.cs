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

        public AccountController(UserManager<ApplicationUser> userManager ,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService,
            IConfiguration configuration,
            SignInManager<ApplicationUser> signInManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._emailService = emailService;
            this._configuration = configuration;
            this._signInManager = signInManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser , string role)
        {
            //Check Exist User
            var existuser = await _userManager.FindByEmailAsync(registerUser.Email);
            if(existuser != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error", Message = "User Already Exist!." });
            }
            var existUserByUsername = await _userManager.FindByNameAsync(registerUser.UserName);
            if (existUserByUsername != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error", Message = "Username is already taken!." });
            }
            var existUserByNationalId = await _userManager.Users
                .FirstOrDefaultAsync(u => u.NationalId == registerUser.NationalId);

            if (existUserByNationalId != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error", Message = "National ID is already registered!." });
            }

            //Add User To DB
            ApplicationUser user = new() 
            { 
                Email=registerUser.Email,
                UserName=registerUser.UserName,
                NationalId=registerUser.NationalId,
                SecurityStamp=Guid.NewGuid().ToString(),
                TwoFactorEnabled=true
            
            };
            if(await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(user, registerUser.Password);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new Response { Status = "Error", Message = "User Failed To Created!." });


                }
                //Add Role To user 
                await _userManager.AddToRoleAsync(user, role);

                //Add Verify Token Email 
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationlink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
                var message = new Message(new string[] { user.Email! }, "Confirmation Email Link", confirmationlink!);
                _emailService.SendEmail(message);

                return   StatusCode(StatusCodes.Status200OK,
                        new Response { Status = "Success", Message = $"User Created Successfully. & Email Sent To {user.Email} Successfully." });
            }
            else
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                   new Response { Status = "Error", Message = "This Role Is Not Exist!." });
            }





          
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
                   new Response { Status = "Success", Message = "Email Verified Successfully." });
                }

            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                  new Response { Status = "Error", Message = "This Use Don't Exist." });

        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] SignInDTO signInDTO)
        {

            var user = await _userManager.FindByEmailAsync(signInDTO.Email);
            if (user.TwoFactorEnabled)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user, signInDTO.Password, false, true);
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                var message = new Message(new string[] { user.Email! }, "OTP Confrimation", token);
                _emailService.SendEmail(message);

                return StatusCode(StatusCodes.Status200OK,
                 new Response { Status = "Success", Message = $"We have sent an OTP to your Email {user.Email}" });
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
                return Ok(new { token=new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expiration=jwtToken.ValidTo });


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
