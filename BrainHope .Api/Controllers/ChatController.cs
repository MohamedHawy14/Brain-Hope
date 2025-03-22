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
                Image = null // Default value
            };

            // Handle image upload
            if (messageDto.Image != null)
            {
                try
                {
                    chatMessage.Image = await ImageHelper.SaveImageAsync(messageDto.Image);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            var savedMessage = await _unitOfWork.ChatRepository.SendMessage(chatMessage);
            await _unitOfWork.Complete();

            var response = new
            {
                senderId = savedMessage.SenderId,
                receiverId = savedMessage.ReceiverId,
                message = savedMessage.Message,
                image = savedMessage.Image // Full URL now returned
            };

            return Ok(response);
        }

        [HttpGet("history/{userid1}/{userid2}")]
        public async Task<IActionResult> GetChatHistory(string user1, string user2)
        {
            var messages = await _unitOfWork.ChatRepository.GetChatHistory(user1, user2);
            var dtoList = messages.Select(m => new
            {
                senderId = m.SenderId,
                receiverId = m.ReceiverId,
                message = m.Message,
                time = m.Time,
                image = m.Image,
               
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
            var messages = await _unitOfWork.ChatRepository.GetAllMessagesForUser(userId);

            var contactGroups = messages.GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId).ToList();
            var contactIds = contactGroups.Select(g => g.Key).Distinct().ToList();

            string baseUrl = "https://braincancer.runasp.net"; // Change this to your domain

            var users = await _userManager.Users
                .Where(u => contactIds.Contains(u.Id))
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    ProfilePhoto = !string.IsNullOrEmpty(u.ProfilePhoto) ? $"{baseUrl}{u.ProfilePhoto}" : null
                })
                .ToListAsync();

            var contacts = contactGroups.Select(g =>
            {
                var lastMessage = g.OrderByDescending(m => m.Time).First();
                var contactUser = users.FirstOrDefault(u => u.Id == g.Key);
                return new ChatContactDTO
                {
                    ContactId = g.Key,
                    LastMessage = lastMessage.Message,
                    LastMessageTime = lastMessage.Time,
                    ContactUserName = contactUser?.UserName ?? g.Key,
                    ProfilePhoto = contactUser?.ProfilePhoto // Ensure full URL
                };
            }).ToList();

            return Ok(contacts);
        }


    }
}
