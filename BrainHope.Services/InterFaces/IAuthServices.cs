using BrainHope.Services.DTO.Authentication.SingUp;
using BrainHope.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainHope.Services.DTO.Authentication.User;
using BrainHope.DataAcess.Models;

namespace BrainHope.Services.InterFaces
{
    public interface IAuthServices
    {
        Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(RegisterUser registerUser);

        Task<ApiResponse<List<string>>> AssignRoleToUserAsync(List<string> roles, ApplicationUser user);
    }
}
