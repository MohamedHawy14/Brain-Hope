using BrainHope.DataAcess.Contexts;
using BrainHope.DataAcess.Models.Chat;
using BrainHope.DataAcess.Repositry.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.DataAcess.Repositry
{
    public class ChatRepository : IChatRepository
    {
        private readonly BrainHopeDbContext _context;

        public ChatRepository(BrainHopeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChatMessage>> GetChatHistory(string user1, string user2)
        {
            return await _context.ChatMessages
                .Where(m => ((m.SenderId == user1 && m.ReceiverId == user2) ||
                             (m.SenderId == user2 && m.ReceiverId == user1)) && !m.Deleted)
                .OrderBy(m => m.Time)
                .ToListAsync();
        }

        public async Task<ChatMessage> SendMessage(ChatMessage message)
        {
            await _context.ChatMessages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task MarkAsRead(string senderId, string receiverId)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.Read)
                .ToListAsync();

            foreach (var msg in messages)
            {
                msg.Read = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMessage(int messageId)
        {
            var message = await _context.ChatMessages.FindAsync(messageId);
            if (message != null)
            {
                message.Deleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddUserConnection(UserConnection connection)
        {
            await _context.UserConnections.AddAsync(connection);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserConnection(string connectionId)
        {
            var connection = await _context.UserConnections.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);
            if (connection != null)
            {
                _context.UserConnections.Remove(connection);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string?> GetUserConnectionId(string userId)
        {
            return await _context.UserConnections
                .Where(c => c.UserId == userId)
                .Select(c => c.ConnectionId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetAllMessagesForUser(string userId)
        {
            return await _context.ChatMessages
                .Where(m => (m.SenderId == userId || m.ReceiverId == userId) && !m.Deleted)
                .ToListAsync();
        }
    }
}
