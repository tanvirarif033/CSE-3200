using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using CSE3200.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CSE3200.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDisasterService _disasterService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IDisasterService disasterService,
            ILogger<HomeController> logger)
        {
            _disasterService = disasterService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var approvedDisasters = _disasterService.GetApprovedDisasters();
                return View(approvedDisasters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading approved disasters");
                // Return empty list or handle error appropriately
                return View(new List<Disaster>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}