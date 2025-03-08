using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.Profile
{
    public class UpdateProfilePostDTO:UpdateProfileDTO
    {
      
        public IFormFile? ProfilePhoto { get; set; }
    }
}
