using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Models.AiModel
{
    public class BrainScanResult:ModelBase
    {
    

        [Required]
        public string UserId { get; set; } // FK to ApplicationUser.Id

        [ForeignKey("PatientId")]
        public ApplicationUser User { get; set; }

        [Required]
        public string ImageName { get; set; }

        [Required]
        public string PredictionResult { get; set; }

        public DateTime ScanDate { get; set; } = DateTime.UtcNow;
    }

}
