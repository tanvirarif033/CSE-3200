using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Security.Claims;
using CSE3200.Application.Features.Chat.Commands;
using CSE3200.Application.Features.Chat.Queries;

namespace CSE3200.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID claim not found");
            }
            return Guid.Parse(userIdClaim);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var userId = GetUserId();
            var isAdmin = User.IsInRole("Admin");

            var messageId = await _mediator.Send(new CreateChatMessageCommand
            {
                SenderId = userId,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                IsFromAdmin = isAdmin
            });

            return Ok(new { MessageId = messageId });
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages([FromQuery] Guid? userId)
        {
            var currentUserId = GetUserId();
            var isAdmin = User.IsInRole("Admin");

            // If admin wants to see messages with a specific user
            Guid? targetUserId = isAdmin ? userId : null;

            var messages = await _mediator.Send(new GetUserMessagesQuery
            {
                UserId = currentUserId,
                OtherUserId = targetUserId
            });

            return Ok(messages);
        }

        [HttpGet("admin/conversations")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAdminConversations()  // Remove 'async' and change return type
        {
            // This will return all unique users who have chatted with admin
            // For now, return empty array - we'll implement this later
            return Ok(new { users = Array.Empty<object>() });
        }
    }

    public class SendMessageRequest
    {
        public Guid? ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}