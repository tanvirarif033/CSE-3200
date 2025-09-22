using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Security.Claims;
using CSE3200.Application.Features.Chat.Commands;
using CSE3200.Application.Features.Chat.Queries;

namespace CSE3200.Web.Controllers
{
    [Authorize]
    public class ChatController : Controller
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

        // Main chat page - handles both admin and user views
        [HttpGet]
        [Route("Chat", Name = "ChatPage")]
        [HttpGet]
        public IActionResult Index()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            ViewBag.CurrentUserId = currentUserId;
            ViewBag.IsAdmin = isAdmin;

            // Redirect admin to admin area chat dashboard
            if (isAdmin)
            {
                return RedirectToAction("Index", "Chat", new { area = "Admin" });
            }

            // Regular users stay on this page
            return View();
        }

        // API endpoint for sending messages
        [HttpPost("Chat/Send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = User.IsInRole("Admin");

                // If user is not admin and doesn't specify receiver, send to default admin
                if (!isAdmin && !request.ReceiverId.HasValue)
                {
                    // Use a default admin ID or get from configuration
                    // For now, we'll require receiverId for users
                    return BadRequest(new { error = "Receiver ID is required" });
                }

                var messageId = await _mediator.Send(new CreateChatMessageCommand
                {
                    SenderId = userId,
                    ReceiverId = request.ReceiverId,
                    Content = request.Content,
                    IsFromAdmin = isAdmin
                });

                return Ok(new
                {
                    MessageId = messageId,
                    Success = true,
                    Message = "Message sent successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // API endpoint for getting messages
        [HttpGet("Chat/GetMessages")]
        public async Task<IActionResult> GetMessages([FromQuery] Guid? userId)
        {
            try
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
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // API endpoint for getting user's conversations (for admin)
        [HttpGet("Chat/GetConversations")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetConversations()
        {
            try
            {
                var currentUserId = GetUserId();

                var messages = await _mediator.Send(new GetUserMessagesQuery
                {
                    UserId = currentUserId,
                    OtherUserId = null // Get all conversations
                });

                // Group messages by user for conversation list
                var conversations = messages
                    .GroupBy(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                    .Where(g => g.Key.HasValue)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        LastMessage = g.OrderByDescending(m => m.SentAt).First(),
                        UnreadCount = g.Count(m => !m.IsRead && m.SenderId != currentUserId)
                    })
                    .OrderByDescending(c => c.LastMessage.SentAt)
                    .ToList();

                return Ok(conversations);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // API endpoint to get default admin ID for users
        [HttpGet("Chat/GetAdminInfo")]
        public IActionResult GetAdminInfo()
        {
            // In a real application, you might get this from a configuration or database
            // For now, return a default admin ID or implement your logic here
            return Ok(new
            {
                DefaultAdminId = "29671297-6bd9-476a-f172-08ddddda291f",
                SupportEmail = "support@cse3200.com",
                SupportHours = "24/7"
            });
        }

        // API endpoint to mark messages as read
        [HttpPost("Chat/MarkAsRead")]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadRequest request)
        {
            try
            {
                // Implement message read status update logic here
                // This would require a new command in your MediatR setup
                return Ok(new { Success = true, Message = "Messages marked as read" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class SendMessageRequest
    {
        public Guid? ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class MarkAsReadRequest
    {
        public List<Guid> MessageIds { get; set; } = new List<Guid>();
        public Guid? ConversationUserId { get; set; }
    }
}