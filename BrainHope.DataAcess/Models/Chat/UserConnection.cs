using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Models.Chat
{
    public class UserConnection
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } 
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } 
        public string ConnectionId { get; set; } 
    }
}
