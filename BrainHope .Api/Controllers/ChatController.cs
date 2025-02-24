using BrainHope.DataAcess.Models;
using BrainHope.DataAcess.Models.Chat;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.Services.DTO.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Utilites;

namespace BrainHope_.Api.Controllers
{
    [Route("Chat/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly List<string> _allowedExtensions = new List<string> { ".jpg", ".png" };
        private readonly long _maxAllowedImageSize = 3145728; // 3MB

        public ChatController(IUnitOfWork unitOfWork , UserManager<ApplicationUser> userManager )
        {
            _unitOfWork = unitOfWork;
            this._userManager = userManager;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromForm] ChatMessageDTO messageDto)
        {
            if (messageDto == null)
                return BadRequest("Invalid message data.");

     
            var chatMessage = new ChatMessage
            {
                SenderId = messageDto.SenderId,
                ReceiverId = messageDto.ReceiverId,
                Message = messageDto.Message,
                MessageType = SD.Message_Text 
                
            };

            // If a file is provided, validate and convert it to byte[]
            if (messageDto.image != null)
            {
                // Validate file extension.
                var ext = Path.GetExtension(messageDto.image.FileName).ToLower();
                if (!_allowedExtensions.Contains(ext))
                {
                    return BadRequest("Only .jpg & .png files are allowed.");
                }

                // Validate file size.
                if (messageDto.image.Length > _maxAllowedImageSize)
                {
                    return BadRequest("Max allowed size is 3MB.");
                }

                using var dataStream = new MemoryStream();
                await messageDto.image.CopyToAsync(dataStream);
                chatMessage.Image = dataStream.ToArray();

                chatMessage.MessageType = SD.Message_Image;
            }

          
            var savedMessage = await _unitOfWork.ChatRepository.SendMessage(chatMessage);
            await _unitOfWork.Complete();

        
            var response = new
            {
                senderId = savedMessage.SenderId,
                receiverId = savedMessage.ReceiverId,
                message = savedMessage.Message,
                messageType = savedMessage.MessageType,
                image=savedMessage.Image
            };

            return Ok(response);
        }

        [HttpGet("history/{user1}/{user2}")]
        public async Task<IActionResult> GetChatHistory(string user1, string user2)
        {
            var messages = await _unitOfWork.ChatRepository.GetChatHistory(user1, user2);
            var dtoList = messages.Select(m => new
            {
                senderId = m.SenderId,
                receiverId = m.ReceiverId,
                message = m.Message,
                time = m.Time,
                messageType = m.MessageType,
                image = m.Image,
                //read = m.Read,
                //deleted = m.Deleted
            });
            return Ok(dtoList);
        }

        [HttpPut("read/{senderId}/{receiverId}")]
        public async Task<IActionResult> MarkMessagesAsRead(string senderId, string receiverId)
        {
            await _unitOfWork.ChatRepository.MarkAsRead(senderId, receiverId);
            await _unitOfWork.Complete();
            return NoContent();
        }

        [HttpDelete("delete/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            await _unitOfWork.ChatRepository.DeleteMessage(messageId);
            await _unitOfWork.Complete();
            return NoContent();
        }

        [HttpGet("contacts/{userId}")]
        public async Task<IActionResult> GetChatContacts(string userId)
        {
            // Get all messages for the user (sent or received)
            var messages = await _unitOfWork.ChatRepository.GetAllMessagesForUser(userId);

            // Group messages by the contact id (if current user is sender then contact is receiver, else sender)
            var contactGroups = messages.GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId).ToList();
            var contactIds = contactGroups.Select(g => g.Key).Distinct().ToList();

            // Use UserManager to fetch user details from AspNetUsers
            var users = await _userManager.Users
                .Where(u => contactIds.Contains(u.Id))
                .ToListAsync();

            var contacts = contactGroups.Select(g =>
            {
                var lastMessage = g.OrderByDescending(m => m.Time).First();
                // Find the user corresponding to the contact id
                var contactUser = users.FirstOrDefault(u => u.Id == g.Key);
                return new ChatContactDTO
                {
                    ContactId = g.Key,
                    LastMessage = lastMessage.Message,
                    LastMessageTime = lastMessage.Time,
                    ContactUserName = contactUser?.UserName ?? g.Key
                };
            }).ToList();

            return Ok(contacts);
        }
    

}
}
