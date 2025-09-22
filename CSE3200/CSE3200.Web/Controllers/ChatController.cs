using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Security.Claims;
using CSE3200.Application.Features.Chat.Commands;
using CSE3200.Application.Features.Chat.Queries;

namespace CSE3200.Web.Controllers
{
    [Authorize]
    [Route("[controller]")] // CHANGED: Removed "api/" from the route
    public class ChatController : Controller // Changed from ControllerBase
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

        // Handles /Chat requests - serves HTML page
        [HttpGet]
        [Route("", Name = "ChatPage")]
        public IActionResult ChatPage()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ViewBag.CurrentUserId = currentUserId;
            return View("~/Views/Chat/Index.cshtml");
        }

        // API endpoints - note the [Route("api/...")] prefix
        [HttpPost("api/send")]
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

        [HttpGet("api/messages")]
        public async Task<IActionResult> GetMessages([FromQuery] Guid? userId)
        {
            var currentUserId = GetUserId();
            var isAdmin = User.IsInRole("Admin");

            Guid? targetUserId = isAdmin ? userId : null;

            var messages = await _mediator.Send(new GetUserMessagesQuery
            {
                UserId = currentUserId,
                OtherUserId = targetUserId
            });

            return Ok(messages);
        }

        [HttpGet("api/admin/conversations")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAdminConversations()
        {
            return Ok(new { users = Array.Empty<object>() });
        }
    }

    public class SendMessageRequest
    {
        public Guid? ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}