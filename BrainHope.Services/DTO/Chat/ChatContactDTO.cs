using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.DTO.Chat
{
    public class ChatContactDTO
    {
        public string ContactId { get; set; }
        public string ContactUserName { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }
    }
}
