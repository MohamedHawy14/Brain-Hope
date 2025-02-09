using BrainHope.DataAcess.Models;
using BrainHope.Services.DTO;
using BrainHope.Services.DTO.Authentication.SingUp;
using BrainHope.Services.DTO.Authentication.User;
using BrainHope.Services.DTO.Email;
using BrainHope.Services.InterFaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthServices(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService,
            IConfiguration configuration,
            SignInManager<ApplicationUser> signInManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
        }

        public async Task<ApiResponse<List<string>>> AssignRoleToUserAsync(List<string> roles, ApplicationUser user)
        {
            var assignedRole = new List<string>();
            foreach (var role in roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    if (!await _userManager.IsInRoleAsync(user, role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                        assignedRole.Add(role);
                    }
                }
            }

            return new ApiResponse<List<string>>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Roles has been assigned"
            ,
                Response = assignedRole
            };
        }

        public async Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(RegisterUser registerUser)
        {
            //Check Exist User
            var existuser = await _userManager.FindByEmailAsync(registerUser.Email);
            if (existuser != null)
            {
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 403, Message = "User already exists!" };
            }
            var existUserByUsername = await _userManager.FindByNameAsync(registerUser.UserName);
            if (existUserByUsername != null)
            {
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 403, Message = "Username is already taken!" };
          
            }
            var existUserByNationalId = await _userManager.Users
                .FirstOrDefaultAsync(u => u.NationalId == registerUser.NationalId);

            if (existUserByNationalId != null)
            {
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 403, Message = "National ID is already registered!" };

            }

            //Add User To DB
            ApplicationUser user = new()
            {
                Email = registerUser.Email,
                UserName = registerUser.UserName,
                NationalId = registerUser.NationalId,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = true

            };
            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                return new ApiResponse<CreateUserResponse> { Response = new CreateUserResponse() { User = user, Token = token }, IsSuccess = true, StatusCode = 201, Message = "User Created Successfully ." };

            }
            else
            {
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 500, Message = "User Failed to Create" };

            }

        }
    }
}
