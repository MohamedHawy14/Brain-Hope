using BrainHope.DataAcess.Models;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.Services.DTO.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Utilites;

namespace BrainHope_.Api.Controllers
{
    [Route("profile/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
       
        private readonly UserManager<ApplicationUser> _userManager;
       

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
          
            this._userManager = userManager;
        }

        [HttpPost("CompleteUrProfile")]
        public async Task<IActionResult> CompleteUrProfile([FromForm] string userId, [FromForm] CompleteprofileDTO completeProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update profile details
            user.Description = completeProfile.Bio;
            user.Address = completeProfile.Address;
            user.PhoneNumber = completeProfile.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                // Optionally return the updated profile
                var profileDTO = new UserProfileDTO
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    ProfilePhoto = string.IsNullOrEmpty(user.ProfilePhoto) ? null : $"{baseUrl}{user.ProfilePhoto}",
                    Bio = user.Description,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber
                };

                return Ok(new { message = "Profile updated successfully.", profile = profileDTO });
            }
            else
            {
                // Return errors if update fails.
                return StatusCode(500, new { message = "Profile update failed.", errors = result.Errors });
            }
        }


        [HttpGet("GetUserProfile")] //Patient & Admin & Doctor 

        public async Task< IActionResult> GetUserProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";


            var profileDTO = new UserProfileDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                ProfilePhoto = string.IsNullOrEmpty(user.ProfilePhoto) ? null : $"{baseUrl}{user.ProfilePhoto}",
                Bio = user.Description,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };

            return Ok(profileDTO);
        }

        [HttpGet("EditProfile")]  // will don't use because that similar to GetUserProfile
        public async Task<IActionResult> EditProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var profileDTO = new UpdateProfileGetDTO
            {
                UserName = user.UserName,
                ProfilePhoto = string.IsNullOrEmpty(user.ProfilePhoto) ? null : $"{baseUrl}{user.ProfilePhoto}",
                Bio = user.Description,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };

            return Ok(profileDTO);
        }


        [HttpPut("EditProfile")]
        public async Task<IActionResult> EditProfile([FromQuery] string userId, [FromForm] UpdateProfilePostDTO updateProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }


            #region Update only provided fields, keep others unchanged
            if (!string.IsNullOrWhiteSpace(updateProfile.UserName))
            {
                user.UserName = updateProfile.UserName;
            }

            if (!string.IsNullOrWhiteSpace(updateProfile.Bio))
            {
                user.Description = updateProfile.Bio;
            }

            if (!string.IsNullOrWhiteSpace(updateProfile.Address))
            {
                user.Address = updateProfile.Address;
            }

            if (!string.IsNullOrWhiteSpace(updateProfile.PhoneNumber))
            {
                user.PhoneNumber = updateProfile.PhoneNumber;
            } 
            #endregion

            // Handle Profile Photo Upload
            if (updateProfile.ProfilePhoto != null)
            {
                string photoUrl = await ImageHelper.SaveImageAsync(updateProfile.ProfilePhoto);
                user.ProfilePhoto = photoUrl; // Store full URL in DB
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return StatusCode(500, new { message = "Profile update failed.", errors });
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var updatedProfile = new UserProfileDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                ProfilePhoto = string.IsNullOrEmpty(user.ProfilePhoto) ? null : $"{baseUrl}{user.ProfilePhoto}",
                Bio = user.Description,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };

            return Ok(new { message = "Profile updated successfully.", profile = updatedProfile });
        }






    }
}
