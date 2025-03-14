using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO
{
    public class SetDoctorRatingDTO
    {
        [Required(ErrorMessage = "DoctorUserId is required.")]
        public string DoctorUserId { get; set; }

        [Required(ErrorMessage = "Rate is required.")]
        [Range(1, 5, ErrorMessage = "Rate must be between 1 and 5.")]
        public int Rate { get; set; }
    }
}
