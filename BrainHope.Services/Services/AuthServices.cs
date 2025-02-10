using BrainHope.DataAcess.Models;
using BrainHope.Services.DTO;
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

namespace BrainHope.Services.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthServices(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService,
            IConfiguration configuration,
            SignInManager<ApplicationUser> signInManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._configuration = configuration;
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

        public async Task<ApiResponse<LoginOtpResponse>> GetOtpByLoginAsync(SignInDTO signInDTO)
        {
            var user = await _userManager.FindByEmailAsync(signInDTO.Email);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user, signInDTO.Password, false, true);
                if (user.TwoFactorEnabled)
                {
                    var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                    return new ApiResponse<LoginOtpResponse>
                    {
                        Response = new LoginOtpResponse()
                        {
                            User = user,
                            Token = token,
                            IsTwoFactorEnable = user.TwoFactorEnabled
                        },
                        IsSuccess = true,
                        StatusCode = 200,
                        Message = $"OTP send to the email {user.Email}"
                    };

                }
                else
                {
                    return new ApiResponse<LoginOtpResponse>
                    {
                        Response = new LoginOtpResponse()
                        {
                            User = user,
                            Token = string.Empty,
                            IsTwoFactorEnable = user.TwoFactorEnabled
                        },
                        IsSuccess = true,
                        StatusCode = 200,
                        Message = $"2FA is not enabled"
                    };
                }
            }
            else
            {
                return new ApiResponse<LoginOtpResponse>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = $"User doesn't exist."
                };
            }
        }

        public async Task<ApiResponse<LoginResponse>> LoginUserWithJWTokenAsync(string otp, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", otp, false, false);
            if (signIn.Succeeded)
            {
                if (user != null)
                {
                    return await GetJwtTokenAsync(user);
                }
            }
            return new ApiResponse<LoginResponse>()
            {

                Response = new LoginResponse()
                {

                },
                IsSuccess = false,
                StatusCode = 400,
                Message = $"Invalid Otp"
            };
        }

        public async Task<ApiResponse<LoginResponse>> RenewAccessTokenAsync(LoginResponse tokens)
        {
            if (tokens?.AccessToken == null || string.IsNullOrEmpty(tokens.AccessToken.Token))
            {
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Access token is missing or invalid"
                };
            }

            if (tokens?.RefreshToken == null || string.IsNullOrEmpty(tokens.RefreshToken.Token))
            {
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Refresh token is missing or invalid"
                };
            }

            ClaimsPrincipal principal;
            try
            {
                principal = GetClaimsPrincipal(tokens.AccessToken.Token);
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = $"Invalid access token: {ex.Message}"
                };
            }

            var email = principal?.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Invalid token: Email not found"
                };
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.RefreshToken != tokens.RefreshToken.Token || tokens.RefreshToken.ExpiryTokenDate <= DateTime.UtcNow)
            {
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Invalid or expired token"
                };
            }

            var response = await GetJwtTokenAsync(user);
            return response;
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
