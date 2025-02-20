using BrainHope.DataAcess.Models.Chat;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.Services.DTO.Chat;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainHope.Services.Hubs
{
    public class ChatHub:Hub
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

        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            var chatMessage = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message
            };

            // Save the message to the database via repository
            await _chatRepository.SendMessage(chatMessage);

            // Get the receiver's connection ID
            var connectionId = await _chatRepository.GetUserConnectionId(receiverId);
            if (connectionId != null)
            {
                // Create the DTO to send over SignalR
                var dto = new ChatMessageDTO
                {
                    SenderId = chatMessage.SenderId,
                    ReceiverId = chatMessage.ReceiverId,
                    Message = chatMessage.Message
                };

                await Clients.Client(connectionId).SendAsync("ReceiveMessage", dto);
            }
        }


    }
}
