using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.AiModel
{
    public class BrainScanImageDTO
    {
        [Required]
        [Display(Name = "image")]
        public IFormFile Image { get; set; }
    }
}
