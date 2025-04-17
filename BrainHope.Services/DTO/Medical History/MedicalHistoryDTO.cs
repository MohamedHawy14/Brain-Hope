using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.Medical_History
{
    public class MedicalHistoryDTO
    {
        public string ChronicDiseases { get; set; }
        public string Allergies { get; set; }
        public string Surgeries { get; set; }
        public string CurrentMedications { get; set; }
        public string FamilyHistory { get; set; }
        public bool IsSmoker { get; set; }
        public bool DrinksAlcohol { get; set; }
        public string? Notes { get; set; }
    }
}
