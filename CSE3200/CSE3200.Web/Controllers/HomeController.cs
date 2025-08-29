using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using CSE3200.Web.Areas.Admin.Models;
using CSE3200.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;

namespace CSE3200.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDisasterService _disasterService;
        private readonly IDonationService _donationService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IDisasterService disasterService,
            IDonationService donationService,
            ILogger<HomeController> logger)
        {
            _disasterService = disasterService;
            _donationService = donationService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var approvedDisasters = _disasterService.GetApprovedDisasters();

                // Calculate total donations for stats
                decimal totalDonations = 0;
                int totalDonors = 0;
                var recentDonations = new List<Donation>();

                foreach (var disaster in approvedDisasters)
                {
                    totalDonations += _donationService.GetTotalDonationsByDisaster(disaster.Id);
                }

                // Get total unique donors and recent donations
                var allDonations = _donationService.GetDonations(1, 1000, "DonationDate DESC", new DataTablesSearch()).data;
                totalDonors = allDonations
                    .Where(d => d.PaymentStatus == "Completed")
                    .Select(d => d.DonorEmail)
                    .Distinct()
                    .Count();

                recentDonations = _donationService.GetRecentDonations(5).ToList();

                ViewBag.TotalDonations = totalDonations;
                ViewBag.TotalDonors = totalDonors;
                ViewBag.RecentDonations = recentDonations;

                return View(approvedDisasters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading approved disasters");
                ViewBag.TotalDonations = 0;
                ViewBag.TotalDonors = 0;
                ViewBag.RecentDonations = new List<Donation>();
                return View(new List<Disaster>());
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Donate(DonationModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Please fix validation errors",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var userName = User.Identity.Name;

                // Use provided details or user account details
                var donorName = !string.IsNullOrEmpty(model.DonorName) ? model.DonorName : userName;
                var donorEmail = !string.IsNullOrEmpty(model.DonorEmail) ? model.DonorEmail : userEmail;

                var donation = new Donation
                {
                    Id = Guid.NewGuid(),
                    DisasterId = model.DisasterId,
                    DonorUserId = userId,
                    DonorName = donorName,
                    DonorEmail = donorEmail,
                    DonorPhone = model.DonorPhone,
                    Amount = model.Amount,
                    PaymentMethod = Enum.Parse<PaymentMethod>(model.PaymentMethod),
                    PaymentStatus = "Pending",
                    DonationDate = DateTime.UtcNow,
                    Notes = model.Notes
                };

                _donationService.AddDonation(donation);

                // Simulate payment processing
                var transactionId = $"TXN_{DateTime.UtcNow:yyyyMMddHHmmss}_{donation.Id.ToString().Substring(0, 8)}";

                // Simulate successful payment
                _donationService.UpdatePaymentStatus(donation.Id, transactionId, "Completed");

                _logger.LogInformation("Donation successful: {DonationId}, Amount: {Amount}, Method: {Method}",
                    donation.Id, donation.Amount, donation.PaymentMethod);

                return Json(new
                {
                    success = true,
                    message = "Donation successful! Thank you for your support.",
                    donationId = donation.Id,
                    transactionId = transactionId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing donation for disaster {DisasterId}", model.DisasterId);
                return Json(new { success = false, message = "Error processing donation. Please try again." });
            }
        }
        [Authorize]
        public IActionResult DonationHistory()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "User not found. Please log in again.";
                    return RedirectToAction("Login", "Account");
                }

                var donations = _donationService.GetDonationsByUser(userId);

                ViewBag.TotalDonated = donations
                    .Where(d => d.PaymentStatus == "Completed")
                    .Sum(d => d.Amount);

                ViewBag.TotalDonations = donations.Count;

                return View(donations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading donation history for user");
                TempData["ErrorMessage"] = "Error loading donation history";
                return View(new List<Donation>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            try
            {
                // Get some statistics for the about page
                var totalDisasters = _disasterService.GetApprovedDisasters().Count;
                var totalDonations = _donationService.GetDonations(1, 1000, "", new DataTablesSearch()).data
                    .Where(d => d.PaymentStatus == "Completed")
                    .Sum(d => d.Amount);
                var totalDonors = _donationService.GetDonations(1, 1000, "", new DataTablesSearch()).data
                    .Where(d => d.PaymentStatus == "Completed")
                    .Select(d => d.DonorEmail)
                    .Distinct()
                    .Count();

                ViewBag.TotalDisasters = totalDisasters;
                ViewBag.TotalDonations = totalDonations;
                ViewBag.TotalDonors = totalDonors;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading about page statistics");
                ViewBag.TotalDisasters = 0;
                ViewBag.TotalDonations = 0;
                ViewBag.TotalDonors = 0;
                return View();
            }
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult GetDisasterDonations(Guid disasterId)
        {
            try
            {
                var donations = _donationService.GetDonationsByDisaster(disasterId)
                    .Where(d => d.PaymentStatus == "Completed")
                    .ToList();

                var totalDonated = donations.Sum(d => d.Amount);
                var donorCount = donations.Select(d => d.DonorEmail).Distinct().Count();

                return Json(new
                {
                    success = true,
                    totalDonated = totalDonated.ToString("C"),
                    donorCount = donorCount,
                    donationCount = donations.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting disaster donations for {DisasterId}", disasterId);
                return Json(new { success = false, message = "Error loading donation information" });
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetUserDonationStats()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var donations = _donationService.GetDonationsByUser(userId)
                    .Where(d => d.PaymentStatus == "Completed")
                    .ToList();

                var totalDonated = donations.Sum(d => d.Amount);
                var donationCount = donations.Count;
                var lastDonation = donations.OrderByDescending(d => d.DonationDate).FirstOrDefault();

                return Json(new
                {
                    success = true,
                    totalDonated = totalDonated.ToString("C"),
                    donationCount = donationCount,
                    lastDonationDate = lastDonation?.DonationDate.ToString("yyyy-MM-dd"),
                    lastDonationAmount = lastDonation?.Amount.ToString("C")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user donation stats");
                return Json(new { success = false, message = "Error loading donation statistics" });
            }
        }
    }
}