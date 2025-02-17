using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Models
{
    public class Doctor:ModelBase
    {
        public  string? GratuedFrom { get; set; }
        public  string? Description { get; set; }
        [Range(1,5)]
        public int Rate { get; set; }
        // Relation With Application User
        public string? UserId { get; set; }
        public virtual ApplicationUser AppUser { get; set; }

        // Calendly Link
        public string? CalendlyLink { get; set; }

        // Many-to-Many Relationship with Patients
        public virtual ICollection<DoctorPatient> DoctorPatients { get; set; } = new HashSet<DoctorPatient>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
    }
}
