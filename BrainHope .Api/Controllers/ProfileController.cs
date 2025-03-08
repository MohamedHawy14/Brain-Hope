using BrainHope.DataAcess.Models;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.Services.DTO.Profile;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BrainHope_.Api.Controllers
{
    [Route("profile/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
       
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly List<string> _allowedExtensions = new List<string> { ".jpg", ".png" };
        private readonly long _maxAllowedImageSize = 3145728;

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
                // Optionally return the updated profile
                var profileDTO = new UserProfileDTO
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    ProfilePhoto = user.ProfilePhoto ?? new byte[0],
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

            
            var profileDTO = new UserProfileDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                ProfilePhoto = user.ProfilePhoto ?? new byte[0],
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

            var profileDTO = new UpdateProfileGetDTO
            {
                UserName = user.UserName,
                ProfilePhoto = user.ProfilePhoto ?? new byte[0],
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

            
            user.UserName = updateProfile.UserName;
            user.Description = updateProfile.Bio;
            user.Address = updateProfile.Address;
            user.PhoneNumber = updateProfile.PhoneNumber;

            // If a new profile photo is provided, validate and update.
            if (updateProfile.ProfilePhoto != null)
            {
                var ext = Path.GetExtension(updateProfile.ProfilePhoto.FileName).ToLower();
                if (!_allowedExtensions.Contains(ext))
                {
                    return BadRequest("Only .jpg & .png files are allowed.");
                }

                if (updateProfile.ProfilePhoto.Length > _maxAllowedImageSize)
                {
                    return BadRequest("Max allowed size is 3MB.");
                }

                using var dataStream = new MemoryStream();
                await updateProfile.ProfilePhoto.CopyToAsync(dataStream);
                user.ProfilePhoto = dataStream.ToArray();
            }

           
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return StatusCode(500, new { message = "Profile update failed.", errors });
            }

           
            var updatedProfile = new UserProfileDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                ProfilePhoto = user.ProfilePhoto ?? new byte[0],
                Bio = user.Description,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };

            return Ok(new { message = "Profile updated successfully.", profile = updatedProfile });
        }






    }
}
