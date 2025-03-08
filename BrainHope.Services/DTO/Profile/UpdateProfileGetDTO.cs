using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.Profile
{
    public class UpdateProfileGetDTO:UpdateProfileDTO
    {
        public byte[]? ProfilePhoto { get; set; }
    }
}
