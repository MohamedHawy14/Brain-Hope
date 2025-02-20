using BrainHope.DataAcess.Models.Chat;
using BrainHope.DataAcess.Repositry.IRepository;
using BrainHope.Services.DTO.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BrainHope_.Api.Controllers
{
    [Route("Chat/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageDTO messageDto)
        {
            if (messageDto == null)
            {
                return BadRequest("Invalid message data.");
            }

            // Map DTO to entity
            var chatMessage = new ChatMessage
            {
                SenderId = messageDto.SenderId,
                ReceiverId = messageDto.ReceiverId,
                Message = messageDto.Message,
            };

            // Save the message
            var savedMessage = await _unitOfWork.ChatRepository.SendMessage(chatMessage);
            await _unitOfWork.Complete();

            // Map saved entity back to DTO for response
            var responseDto = new ChatMessageDTO
            {
                SenderId = savedMessage.SenderId,
                ReceiverId = savedMessage.ReceiverId,
                Message = savedMessage.Message,
            };

            return Ok(responseDto);
        }

        
        [HttpGet("history/{user1}/{user2}")]
        public async Task<IActionResult> GetChatHistory(string user1, string user2)
        {
            var messages = await _unitOfWork.ChatRepository.GetChatHistory(user1, user2);

            // Map each ChatMessage entity to DTO
            var dtoList = messages.Select(m => new ChatMessageDTO
            {
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Message = m.Message,
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
    }
}
