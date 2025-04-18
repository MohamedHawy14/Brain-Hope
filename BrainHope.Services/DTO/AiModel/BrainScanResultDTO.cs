using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.AiModel
{
    public class BrainScanResultDTO
    {
        [Display(Name ="Patient Name")]
        public string PatientName { get; set; }
        public string ImageName { get; set; }
        public string PredictionResult { get; set; }
    }

}
