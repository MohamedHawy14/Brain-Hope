using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.Admin
{
    public class UserCountDTO
    {
        public int TotalUsers { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
    }

}
