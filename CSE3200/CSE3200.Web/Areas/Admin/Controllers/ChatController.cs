using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CSE3200.Infrastructure;
using System.Security.Claims;

namespace CSE3200.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Pass current user ID to view
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ViewBag.CurrentUserId = currentUserId;
            return View();
        }

        [HttpGet("current-user-id")]
        public IActionResult GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(new { userId });
        }
    }
}