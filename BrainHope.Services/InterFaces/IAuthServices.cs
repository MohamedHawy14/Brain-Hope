using BrainHope.Services.DTO.Authentication.SingUp;
using BrainHope.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainHope.Services.DTO.Authentication.User;
using BrainHope.DataAcess.Models;
using BrainHope.Services.DTO.Authentication.SignIn;
using BrainHope.Services.DTO.Admin;

namespace BrainHope.Services.InterFaces
{
    public interface IAuthServices
    {
        Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(RegisterUser registerUser);
        Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAdminAsync(CreateUser createUser);

        Task<ApiResponse<List<string>>> AssignRoleToUserAsync(List<string> roles, ApplicationUser user);
        Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(ApplicationUser user);
        Task<ApiResponse<List<UserDetailsDTO>>> GetAllUsersAsync();




    }
}
