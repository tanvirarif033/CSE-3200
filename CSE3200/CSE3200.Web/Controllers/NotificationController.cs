using CSE3200.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CSE3200.Web.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Identity.Name;
            var notifications = await _notificationService.GetUserNotifications(userId);
            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _notificationService.MarkAsRead(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.Identity.Name;
            await _notificationService.MarkAllAsRead(userId);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.Identity.Name;
            var count = await _notificationService.GetUnreadCount(userId);
            return Json(new { count });
        }
    }
}