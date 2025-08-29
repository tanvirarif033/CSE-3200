using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using CSE3200.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CSE3200.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DonationsController : Controller
    {
        private readonly IDonationService _donationService;
        private readonly IDisasterService _disasterService;
        private readonly ILogger<DonationsController> _logger;

        public DonationsController(
            IDonationService donationService,
            IDisasterService disasterService,
            ILogger<DonationsController> logger)
        {
            _donationService = donationService;
            _disasterService = disasterService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDonationsJson([FromBody] DonationListModel model)
        {
            try
            {
                var (data, total, totalDisplay) = _donationService.GetDonations(
                    model.PageIndex,
                    model.PageSize,
                    model.GetSortExpression(),
                    model.Search);

                return Json(new
                {
                    draw = model.Draw,
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = data.Select(d => new
                    {
                        id = d.Id.ToString(),
                        donorName = d.DonorName,
                        donorEmail = d.DonorEmail,
                        donorPhone = d.DonorPhone,
                        amount = d.Amount.ToString("C"),
                        paymentMethod = d.PaymentMethod.ToString(),
                        paymentStatus = d.PaymentStatus,
                        transactionId = d.TransactionId,
                        donationDate = d.DonationDate.ToString("yyyy-MM-dd HH:mm"),
                        disasterTitle = d.Disaster?.Title ?? "N/A"
                    }).ToArray()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching donations for DataTables");
                return Json(new
                {
                    draw = model.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new object[0]
                });
            }
        }

        public IActionResult Details(Guid id)
        {
            try
            {
                var donation = _donationService.GetDonation(id);
                if (donation == null)
                {
                    return NotFound();
                }

                return View(donation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading donation details");
                TempData["ErrorMessage"] = "Error loading donation details";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Export()
        {
            try
            {
                var donations = _donationService.GetDonations(1, 1000, "DonationDate DESC", new DataTablesSearch()).data;

                // Here you would implement CSV or Excel export logic
                // For now, return a view with all donations
                return View("Export", donations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting donations");
                TempData["ErrorMessage"] = "Error exporting donations";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Statistics()
        {
            try
            {
                var totalDonations = _donationService.GetDonations(1, 1000, "", new DataTablesSearch()).data
                    .Where(d => d.PaymentStatus == "Completed")
                    .Sum(d => d.Amount);

                var recentDonations = _donationService.GetRecentDonations(10);
                var totalDonors = _donationService.GetDonations(1, 1000, "", new DataTablesSearch()).data
                    .Select(d => d.DonorEmail)
                    .Distinct()
                    .Count();

                ViewBag.TotalDonations = totalDonations;
                ViewBag.TotalDonors = totalDonors;
                ViewBag.RecentDonations = recentDonations;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading donation statistics");
                TempData["ErrorMessage"] = "Error loading statistics";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}