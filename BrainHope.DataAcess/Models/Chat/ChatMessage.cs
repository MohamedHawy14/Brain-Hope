using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utilites;

namespace BrainHope.DataAcess.Models.Chat
{
    public class ChatMessage:ModelBase
    {

        [Required]
        public string SenderId { get; set; } // FK from AspNetUsers

        [ForeignKey("SenderId")]
        [JsonIgnore]
        public ApplicationUser Sender { get; set; } 

        [Required]
        public string ReceiverId { get; set; } // FK from AspNetUsers

        [ForeignKey("ReceiverId")]
        [JsonIgnore]
        public ApplicationUser Receiver { get; set; } 

       
        public string? Message { get; set; }

        [Required]
        public DateTime Time { get; set; } = DateTime.UtcNow;

        public bool Read { get; set; } = false; 

        public bool Deleted { get; set; } = false; // Soft Delete

    
        //// "text" or "image"
        //public string MessageType { get; set; } = SD.Message_Text;

        // When MessageType is "image", this holds the binary data
        public string? Image { get; set; }
    }
}
