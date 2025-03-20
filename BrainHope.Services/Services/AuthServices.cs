using BrainHope.DataAcess.Contexts;
using BrainHope.DataAcess.Models;
using BrainHope.Services.DTO;
using BrainHope.Services.DTO.Admin;
using BrainHope.Services.DTO.Authentication.SignIn;
using BrainHope.Services.DTO.Authentication.SingUp;
using BrainHope.Services.DTO.Authentication.User;
using BrainHope.Services.DTO.Email;
using BrainHope.Services.InterFaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utilites;

namespace BrainHope.Services.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly BrainHopeDbContext _context;
        private new List<string> _allowedextention = new List<string> { ".jpg", ".png" };
        private long _maxallowImagesize = 3145728;

        public AuthServices(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService,
            IConfiguration configuration,
            SignInManager<ApplicationUser> signInManager,
            BrainHopeDbContext context)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._configuration = configuration;
            this._signInManager = signInManager;
            this._context = context;
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

        public async Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAdminAsync(CreateUser createUser)
        {
            var response = new ApiResponse<CreateUserResponse>();

            // Check if the user already exists
            var existingUser = await _userManager.FindByEmailAsync(createUser.Email);
            if (existingUser != null)
            {
                response.IsSuccess = false;
                response.Message = "User with this email already exists.";
                return response;
            }
            var existingUser2 = await _userManager.FindByNameAsync(createUser.UserName);
            if (existingUser2 != null)
            {
                response.IsSuccess = false;
                response.Message = "User with this UserName already exists.";
                return response;
            }
            var existUserByNationalId = await _userManager.Users
                .FirstOrDefaultAsync(u => u.NationalId == createUser.NationalId);
            if (existUserByNationalId != null)
            {
                response.IsSuccess = false;
                response.Message = "User with this National Id already exists.";
                return response;
            }

            if (!_allowedextention.Contains(Path.GetExtension(createUser.ProfilePhoto.FileName).ToLower()))
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 500, Message = "Only .jpg & .png" };
            if (createUser.ProfilePhoto.Length > _maxallowImagesize)
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 500, Message = "Max Allowed Size Is 3Mb" };





            using var datastream = new MemoryStream();
            await createUser.ProfilePhoto.CopyToAsync(datastream);

            // Create a new ApplicationUser object
            var user = new ApplicationUser
            {
                UserName = createUser.UserName,
                Email = createUser.Email,
                NationalId = createUser.NationalId,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = true,
                ProfilePhoto = datastream.ToArray()  // Save profile photo to database
            };

            // Create user in Identity
            var result = await _userManager.CreateAsync(user, createUser.Password);
            if (!result.Succeeded)
            {
                response.IsSuccess = false;
                response.Message = "Failed to create user: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return response;
            }



            // Assign roles to the user
            await AssignRoleToUserAsync(createUser.Roles, user);

            // Handle role-specific logic
            if (createUser.Roles.Contains(SD.Role_Doctor))
            {
                var doctor = new Doctor
                {
                    UserId = user.Id
                };
                _context.Doctors.Add(doctor);
            }
            else if (createUser.Roles.Contains(SD.Role_Patient))
            {
                var patient = new Patient
                {
                    UserId = user.Id
                };
                _context.Patients.Add(patient);
            }
            else
            {
                var admin = new Admin
                {
                    UserId = user.Id
                };
                _context.Admins.Add(admin);

            }

            await _context.SaveChangesAsync();

            // Generate a token for the user
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            response.IsSuccess = true;
            response.Message = "User created successfully. Please confirm your email (=>Spam If needed).";
            response.Response = new CreateUserResponse
            {
                User = user,
                Token = token
            };

            return response;
        }


        public async Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(RegisterUser registerUser)
        {
            // Check if the user already exists
            var existUser = await _userManager.FindByEmailAsync(registerUser.Email);
            if (existUser != null)
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

            if (!_allowedextention.Contains(Path.GetExtension(registerUser.ProfilePhoto.FileName).ToLower()))
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 500, Message = "Only .jpg & .png" };
            if (registerUser.ProfilePhoto.Length > _maxallowImagesize)
                return new ApiResponse<CreateUserResponse> { IsSuccess = false, StatusCode = 500, Message = "Max Allowed Size Is 3Mb" };





            using var datastream = new MemoryStream();
            await registerUser.ProfilePhoto.CopyToAsync(datastream);

            // Create new user object
            ApplicationUser user = new()
            {
                Email = registerUser.Email,
                UserName = registerUser.UserName,
                NationalId = registerUser.NationalId,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = true,
                ProfilePhoto = datastream.ToArray()  // Save profile photo to database
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (!result.Succeeded)
            {
                return new ApiResponse<CreateUserResponse>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // Automatically assign "Patient" role
            if (!await _roleManager.RoleExistsAsync(SD.Role_Patient))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Patient));
            }
            await _userManager.AddToRoleAsync(user, SD.Role_Patient);

            // Insert new record in Patient table
            var patient = new Patient
            {
                UserId = user.Id,
                AppUser = user
            };

            _context.Patients.Add(patient); // Save to Patient table
            await _context.SaveChangesAsync();

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return new ApiResponse<CreateUserResponse>
            {
                Response = new CreateUserResponse
                {
                    User = null,
                    Token = null
                },
                IsSuccess = true,
                StatusCode = 201,
                Message = "User created successfully. Please confirm your email (=>Spam If needed)."
            };
        }
        public async Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(ApplicationUser user)
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

            var jwtToken = GetToken(authClaims); //access token
            var refreshToken = GenerateRefreshToken();
            _ = int.TryParse(_configuration["JWT:RefreshTokenValidity"], out int refreshTokenValidity);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenValidity);

            await _userManager.UpdateAsync(user);

            return new ApiResponse<LoginResponse>
            {
                Response = new LoginResponse()
                {
                    AccessToken = new TokenType()
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        ExpiryTokenDate = jwtToken.ValidTo
                    },
                    RefreshToken = new TokenType()
                    {
                        Token = user.RefreshToken,
                        ExpiryTokenDate = (DateTime)user.RefreshTokenExpiry
                    }
                },

                IsSuccess = true,
                StatusCode = 200,
                Message = $"Token created"
            };
        }


        public async Task<ApiResponse<List<UserDetailsDTO>>> GetAllUsersAsync()
        {
            
            var users = await _userManager.Users.ToListAsync();

            
            var userDtos = users.Select(u => new UserDetailsDTO
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                NationalId = u.NationalId
            }).ToList();

            return new ApiResponse<List<UserDetailsDTO>>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Users retrieved successfully.",
                Response = userDtos
            };
        }


        #region PrivateMethods
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);
            var expirationTimeUtc = DateTime.UtcNow.AddMinutes(tokenValidityInMinutes);
            var localTimeZone = TimeZoneInfo.Local;
            var expirationTimeInLocalTimeZone = TimeZoneInfo.ConvertTimeFromUtc(expirationTimeUtc, localTimeZone);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: expirationTimeInLocalTimeZone,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new Byte[64];
            var range = RandomNumberGenerator.Create();
            range.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetClaimsPrincipal(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken), "Access token cannot be null or empty.");
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            try
            {
                var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);
                return principal;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Invalid token.", ex);
            }
        }
        #endregion
    }
}
