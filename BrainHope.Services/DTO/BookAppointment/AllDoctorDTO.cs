using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.BookAppointment
{
    public class AllDoctorDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Rate { get; set; }
        public string ProfilePhoto { get; set; }
    }
}
