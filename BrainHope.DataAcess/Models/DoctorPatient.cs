using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Models
{
    public class DoctorPatient
    {
        // Composite Primary Key
        public int DoctorId { get; set; }
        public int PatientId { get; set; }

        // Navigation Properties
        public virtual Doctor Doctor { get; set; }
        public virtual Patient Patient { get; set; }
    }

}
