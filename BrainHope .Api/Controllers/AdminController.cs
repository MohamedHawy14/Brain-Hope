using BrainHope.DataAcess.Models;
using BrainHope.Services.DTO;
using BrainHope.Services.DTO.Admin;
using BrainHope.Services.DTO.Email;
using BrainHope.Services.InterFaces;
using BrainHope.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilites;

namespace BrainHope_.Api.Controllers
{
    [Route("Admin/[controller]")]
    [ApiController]
    //[Authorize(Roles =SD.Role_Admin)]
    public class AdminController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthServices _authServices;

        public AdminController(UserManager<ApplicationUser> userManager,
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
        [HttpPost("CreateUserWithRole")]
        public async Task<IActionResult> RegisterUser([FromForm] CreateUser createUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response { Message = "Invalid user data.", IsSuccess = false });
            }

            var tokenResponse = await _authServices.CreateUserWithTokenAdminAsync(createUser);

            if (!tokenResponse.IsSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Message = tokenResponse.Message, IsSuccess = false });
            }

            // Assign roles and create user entry in Doctor or Patient tables
            await _authServices.AssignRoleToUserAsync(createUser.Roles, tokenResponse.Response.User);

            // Generate email confirmation link
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account",
                new { token = tokenResponse.Response.Token, email = createUser.Email }, Request.Scheme);

            var message = new Message(new string[] { createUser.Email! }, "Confirmation Email Link", confirmationLink!);
            _emailService.SendEmail(message);

            return Ok(new Response
            {
                Status = "Success",
                Message = $"User Created Successfully & Email Sent To {createUser.Email} Successfully.",
                IsSuccess = true
            });
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
                   new Response { Status = "Success", Message = "Email Verified Successfully.", IsSuccess = true });
                }

            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                  new Response { Status = "Error", Message = "This Use Don't Exist." });

        }

        [HttpPost("AssignRoleToUser")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleDTO model)
        {
            if (string.IsNullOrEmpty(model.NationalId) || string.IsNullOrEmpty(model.RoleId))
            {
                return BadRequest("National ID and Role ID are required.");
            }

            // Find user by National ID
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.NationalId == model.NationalId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Find role by Role ID
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == model.RoleId);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            // Assign role
            var response = await _authServices.AssignRoleToUserAsync(new List<string> { role.Name }, user);
            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _authServices.GetAllUsersAsync();
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, result.Message);
            }
            return Ok(result.Response);
        }


    }
}
