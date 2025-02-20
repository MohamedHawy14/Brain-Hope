using BrainHope.DataAcess.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Repositry.IRepository
{
    public interface IChatRepository
    {
        Task<IEnumerable<ChatMessage>> GetChatHistory(string user1, string user2);
        Task<ChatMessage> SendMessage(ChatMessage message);
        Task MarkAsRead(string senderId, string receiverId);
        Task DeleteMessage(int messageId);
        Task AddUserConnection(UserConnection connection);
        Task RemoveUserConnection(string connectionId);
        Task<string?> GetUserConnectionId(string userId);
    }
}
