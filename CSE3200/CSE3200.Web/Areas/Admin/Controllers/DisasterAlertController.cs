using CSE3200.Application.Features.DisasterAlerts.Commands;
using CSE3200.Application.Features.DisasterAlerts.Queries;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using CSE3200.Web.Areas.Admin.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSE3200.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DisasterAlertController : Controller
    {
        private readonly IDisasterAlertService _alertService;
        private readonly IMediator _mediator;
        private readonly ILogger<DisasterAlertController> _logger;

        public DisasterAlertController(
            IDisasterAlertService alertService,
            IMediator mediator,
            ILogger<DisasterAlertController> logger)
        {
            _alertService = alertService;
            _mediator = mediator;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View(new AddDisasterAlertModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddDisasterAlertModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var command = new AddDisasterAlertCommand
                {
                    Title = model.Title,
                    Message = model.Message,
                    Severity = model.Severity,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    DisplayOrder = model.DisplayOrder,
                    CreatedBy = User.Identity.Name
                };

                var alertId = await _mediator.Send(command);

                TempData["SuccessMessage"] = "Disaster alert created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating disaster alert");
                ModelState.AddModelError("", $"Error creating disaster alert: {ex.Message}");

#if DEBUG
                ModelState.AddModelError("", $"Inner exception: {ex.InnerException?.Message}");
#endif

                return View(model);
            }
        }

        public IActionResult Edit(Guid id)
        {
            var alert = _alertService.GetAlert(id);
            if (alert == null)
            {
                return NotFound();
            }

            return View(new EditDisasterAlertModel
            {
                Id = alert.Id,
                Title = alert.Title,
                Message = alert.Message,
                Severity = alert.Severity,
                StartDate = alert.StartDate,
                EndDate = alert.EndDate,
                DisplayOrder = alert.DisplayOrder
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditDisasterAlertModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var command = new UpdateDisasterAlertCommand
                {
                    Id = model.Id,
                    Title = model.Title,
                    Message = model.Message,
                    Severity = model.Severity,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    DisplayOrder = model.DisplayOrder,
                    ModifiedBy = User.Identity.Name
                };

                var success = await _mediator.Send(command);

                if (success)
                {
                    TempData["SuccessMessage"] = "Disaster alert updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Disaster alert not found");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating disaster alert");
                ModelState.AddModelError("", $"Error updating disaster alert: {ex.Message}");
                return View(model);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var command = new ToggleDisasterAlertStatusCommand
                {
                    Id = id,
                    ModifiedBy = User.Identity.Name
                };

                var success = await _mediator.Send(command);

                if (success)
                {
                    return Json(new { success = true, message = "Disaster alert status updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update disaster alert status" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling disaster alert status");
                return Json(new { success = false, message = $"Error updating disaster alert status: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _alertService.DeleteAlert(id);
                return Json(new { success = true, message = "Disaster alert deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting disaster alert");
                return Json(new { success = false, message = $"Error deleting disaster alert: {ex.Message}" });
            }
        }


        [HttpPost]
        public JsonResult GetAlertsJson([FromBody] DisasterAlertListModel model)
        {
            _logger.LogInformation("GetAlertsJson called with model: {@Model}", model);

            if (model == null)
            {
                _logger.LogWarning("DisasterAlertListModel is null in GetAlertsJson");
                return Json(new
                {
                    draw = 1,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new object[0]
                });
            }

            try
            {
                // Get alerts with proper sorting
                var alerts = _alertService.GetAllAlertsWithPaging(
                    model.PageIndex,
                    model.PageSize,
                    out int totalCount);

                _logger.LogInformation("Returning {Count} alerts out of {TotalCount}", alerts.Count, totalCount);

                return Json(new
                {
                    draw = model.Draw,
                    recordsTotal = totalCount,
                    recordsFiltered = totalCount,
                    data = alerts.Select(a => new
                    {
                        id = a.Id.ToString(),
                        title = a.Title,
                        severity = a.Severity.ToString(),
                        startDate = a.StartDate?.ToString("yyyy-MM-ddTHH:mm:ss"),
                        endDate = a.EndDate?.ToString("yyyy-MM-ddTHH:mm:ss"),
                        isActive = a.IsActive,
                        displayOrder = a.DisplayOrder,
                        createdDate = a.CreatedDate.ToString("yyyy-MM-ddTHH:mm:ss")
                    }).ToArray()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching disaster alerts for DataTables");
                return Json(new
                {
                    draw = model.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new object[0]
                });
            }
        }

        private string GetActionButtons(DisasterAlert alert)
        {
            var buttons = new System.Text.StringBuilder();

            buttons.Append($"<a href=\"/Admin/DisasterAlert/Edit/{alert.Id}\" class=\"btn btn-sm btn-outline-primary me-1\">");
            buttons.Append("<i class=\"bi bi-pencil-square\"></i> Edit</a>");

            var statusText = alert.IsActive ? "Deactivate" : "Activate";
            var statusClass = alert.IsActive ? "warning" : "success";
            buttons.Append($"<button onclick=\"toggleAlertStatus('{alert.Id}')\" class=\"btn btn-sm btn-outline-{statusClass} me-1\">");
            buttons.Append($"<i class=\"bi bi-power\"></i> {statusText}</button>");

            buttons.Append($"<button onclick=\"confirmDelete('{alert.Id}')\" class=\"btn btn-sm btn-outline-danger\">");
            buttons.Append("<i class=\"bi bi-trash\"></i> Delete</button>");

            return buttons.ToString();
        }
    }
}