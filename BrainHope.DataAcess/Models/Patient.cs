using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Models
{
    public class Patient:ModelBase
    {
        // Relation With Application User
        public string? UserId { get; set; }
        public virtual ApplicationUser AppUser { get; set; }

        // Many-to-Many Relationship with Doctors
        public virtual ICollection<DoctorPatient> DoctorPatients { get; set; } = new List<DoctorPatient>();
    }
}
