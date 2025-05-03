using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Models
{
    public class ModelBase
    {
        [Key]
        public  int Id { get; set; }
    }
}
