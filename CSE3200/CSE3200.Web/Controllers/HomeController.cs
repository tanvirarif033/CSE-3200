using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using CSE3200.Infrastructure.Identity;
using CSE3200.Web.Areas.Admin.Models;
using CSE3200.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CSE3200.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDisasterService _disasterService;
        private readonly IDonationService _donationService;
        private readonly IVolunteerAssignmentService _volunteerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IDisasterService disasterService,
            IDonationService donationService,
            IVolunteerAssignmentService volunteerService,
            UserManager<ApplicationUser> userManager,
            ILogger<HomeController> logger)
        {
            _disasterService = disasterService;
            _donationService = donationService;
            _volunteerService = volunteerService;
            _userManager = userManager;
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

                // Add volunteer count to each disaster
                foreach (var disaster in approvedDisasters)
                {
                    disaster.VolunteerCount = _volunteerService.GetAssignedVolunteerCount(disaster.Id);
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

                // Make sure to include the Disaster navigation property
                var donations = _donationService.GetDonationsByUser(userId)
                    .OrderByDescending(d => d.DonationDate)
                    .ToList();

                // If your service doesn't automatically include Disaster, you may need to load it
                foreach (var donation in donations)
                {
                    if (donation.Disaster == null && donation.DisasterId != Guid.Empty)
                    {
                        donation.Disaster = _disasterService.GetDisaster(donation.DisasterId);
                    }
                }

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

                // Calculate total volunteers across all disasters
                int totalVolunteers = 0;
                var approvedDisasters = _disasterService.GetApprovedDisasters();
                foreach (var disaster in approvedDisasters)
                {
                    totalVolunteers += _volunteerService.GetAssignedVolunteerCount(disaster.Id);
                }

                ViewBag.TotalDisasters = totalDisasters;
                ViewBag.TotalDonations = totalDonations;
                ViewBag.TotalDonors = totalDonors;
                ViewBag.TotalVolunteers = totalVolunteers;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading about page statistics");
                ViewBag.TotalDisasters = 0;
                ViewBag.TotalDonations = 0;
                ViewBag.TotalDonors = 0;
                ViewBag.TotalVolunteers = 0;
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
                var volunteerCount = _volunteerService.GetAssignedVolunteerCount(disasterId);

                return Json(new
                {
                    success = true,
                    totalDonated = totalDonated.ToString("C"),
                    donorCount = donorCount,
                    donationCount = donations.Count,
                    volunteerCount = volunteerCount
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

        [HttpGet]
        public IActionResult GetDisasterVolunteers(Guid disasterId)
        {
            try
            {
                var assignments = _volunteerService.GetDisasterAssignments(disasterId);
                var volunteerCount = assignments.Count;

                return Json(new
                {
                    success = true,
                    volunteerCount = volunteerCount,
                    assignments = assignments.Select(a => new
                    {
                        taskDescription = a.TaskDescription,
                        assignedDate = a.AssignedDate.ToString("yyyy-MM-dd"),
                        assignedBy = a.AssignedBy,
                        status = a.Status
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting disaster volunteers for {DisasterId}", disasterId);
                return Json(new { success = false, message = "Error loading volunteer information" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDisasterDetails(Guid disasterId)
        {
            try
            {
                var disaster = _disasterService.GetDisaster(disasterId);
                if (disaster == null)
                {
                    return Json(new { success = false, message = "Disaster not found" });
                }

                var assignments = _volunteerService.GetDisasterAssignments(disasterId);
                var volunteerCount = assignments.Count;

                // Get volunteer names for each assignment
                var volunteersWithNames = new List<object>();
                foreach (var assignment in assignments)
                {
                    // Get user details for the volunteer
                    var user = await _userManager.FindByIdAsync(assignment.VolunteerUserId);
                    var volunteerName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown Volunteer";

                    volunteersWithNames.Add(new
                    {
                        taskDescription = assignment.TaskDescription,
                        assignedDate = assignment.AssignedDate.ToString("yyyy-MM-dd"),
                        assignedBy = assignment.AssignedBy,
                        status = assignment.Status,
                        volunteerName = volunteerName,
                        volunteerEmail = user?.Email
                    });
                }

                return Json(new
                {
                    success = true,
                    disaster = new
                    {
                        title = disaster.Title,
                        description = disaster.Description,
                        location = disaster.Location,
                        occurredDate = disaster.OccurredDate.ToString("yyyy-MM-dd"),
                        severity = disaster.Severity.ToString(),
                        affectedPeople = disaster.AffectedPeople,
                        requiredAssistance = disaster.RequiredAssistance,
                        volunteerCount = volunteerCount
                    },
                    volunteerCount = volunteerCount,
                    volunteers = volunteersWithNames
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting disaster details for {DisasterId}", disasterId);
                return Json(new { success = false, message = "Error loading disaster details" });
            }
        }

        [HttpGet]
        public IActionResult GetDisasterStats()
        {
            try
            {
                var approvedDisasters = _disasterService.GetApprovedDisasters();

                var stats = approvedDisasters.Select(d => new
                {
                    id = d.Id,
                    title = d.Title,
                    volunteerCount = _volunteerService.GetAssignedVolunteerCount(d.Id),
                    donationAmount = _donationService.GetTotalDonationsByDisaster(d.Id),
                    donationCount = _donationService.GetDonationsByDisaster(d.Id)
                        .Count(d => d.PaymentStatus == "Completed")
                }).ToList();

                return Json(new
                {
                    success = true,
                    stats = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting disaster stats");
                return Json(new { success = false, message = "Error loading disaster statistics" });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserVolunteerAssignments()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var assignments = _volunteerService.GetVolunteerAssignments(userId);
                var activeAssignments = assignments.Where(a => a.Status == "Assigned").ToList();
                var completedAssignments = assignments.Where(a => a.Status == "Completed").ToList();

                // Get disaster details for each assignment
                var assignmentsWithDetails = new List<object>();
                foreach (var assignment in assignments)
                {
                    var disaster = _disasterService.GetDisaster(assignment.DisasterId);
                    assignmentsWithDetails.Add(new
                    {
                        disasterId = assignment.DisasterId,
                        disasterTitle = disaster?.Title ?? "Unknown Disaster",
                        taskDescription = assignment.TaskDescription,
                        assignedDate = assignment.AssignedDate.ToString("yyyy-MM-dd"),
                        status = assignment.Status
                    });
                }

                return Json(new
                {
                    success = true,
                    totalAssignments = assignments.Count,
                    activeAssignments = activeAssignments.Count,
                    completedAssignments = completedAssignments.Count,
                    assignments = assignmentsWithDetails
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user volunteer assignments");
                return Json(new { success = false, message = "Error loading volunteer assignments" });
            }
        }
    }
}