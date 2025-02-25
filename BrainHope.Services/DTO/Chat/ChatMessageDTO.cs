using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilites;

namespace BrainHope.Services.DTO.Chat
{
    public class ChatMessageDTO
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string? Message { get; set; }
        //public string MessageType { get; set; } = SD.Message_Text;

        public IFormFile? image { get; set; }

    }
}
