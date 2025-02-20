using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Models.Chat
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; } // FK from AspNetUsers

        [ForeignKey("SenderId")]
        [JsonIgnore]
        public ApplicationUser Sender { get; set; } // Navigation Property

        [Required]
        public string ReceiverId { get; set; } // FK from AspNetUsers

        [ForeignKey("ReceiverId")]
        [JsonIgnore]
        public ApplicationUser Receiver { get; set; } // Navigation Property

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime Time { get; set; } = DateTime.UtcNow;

        public bool Read { get; set; } = false; // False: Single Check, True: Blue Double Check

        public bool Deleted { get; set; } = false; // Soft Delete
    }
}
