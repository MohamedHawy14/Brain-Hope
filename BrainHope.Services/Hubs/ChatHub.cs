using BrainHope.DataAcess.Models.Chat;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.Services.DTO.Chat;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilites;

namespace BrainHope.Services.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatRepository _chatRepository;

        public ChatHub(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                await _chatRepository.AddUserConnection(new UserConnection { UserId = userId, ConnectionId = Context.ConnectionId });
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _chatRepository.RemoveUserConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        // Sending a message over SignalR
        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            // Create a new chat message entity (text message)
            var chatMessage = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                MessageType = SD.Message_Text
               
            };

            // Save the message in DB
            await _chatRepository.SendMessage(chatMessage);

            // Get receiver connection
            var connectionId = await _chatRepository.GetUserConnectionId(receiverId);
            if (connectionId != null)
            {
                // Prepare DTO to send to client
                var dto = new ChatMessageDTO
                {
                    SenderId = chatMessage.SenderId,
                    ReceiverId = chatMessage.ReceiverId,
                    Message = chatMessage.Message,
                   MessageType=SD.Message_Text
                };
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", dto);
            }
        }
    }
}
