using CSE3200.Application.Features.Disasters.Commands;
using CSE3200.Application.Features.Disasters.Queries;
using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using CSE3200.Web.Areas.Admin.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Web;

namespace CSE3200.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Field Representative")]
    public class DisastersController : Controller
    {
        private readonly IDisasterService _disasterService;
        private readonly IMediator _mediator;
        private readonly ILogger<DisastersController> _logger;

        public DisastersController(
            IDisasterService disasterService,
            IMediator mediator,
            ILogger<DisastersController> logger)
        {
            _disasterService = disasterService;
            _mediator = mediator;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View(new AddDisasterModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddDisasterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var isAdmin = User.IsInRole("Admin");
                var command = new AddDisasterCommand
                {
                    Title = model.Title,
                    Description = model.Description,
                    Location = model.Location,
                    OccurredDate = model.OccurredDate,
                    Severity = model.Severity,
                    AffectedPeople = model.AffectedPeople,
                    RequiredAssistance = model.RequiredAssistance,
                    CreatedBy = User.Identity.Name,
                    IsAdmin = isAdmin
                };

                var disasterId = await _mediator.Send(command);

                TempData["SuccessMessage"] = isAdmin
                    ? "Disaster added successfully"
                    : "Disaster added successfully. Waiting for admin approval.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding disaster");

                // Provide more specific error messages
                if (ex.Message.Contains("database", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.Contains("sql", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("", "Database error occurred. Please contact administrator.");
                }
                else
                {
                    ModelState.AddModelError("", $"Error adding disaster: {ex.Message}");
                }

                return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approvals()
        {
            try
            {
                var pendingApprovals = await _mediator.Send(new GetPendingApprovalsQuery());
                return View(pendingApprovals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading pending approvals");
                TempData["ErrorMessage"] = "Error loading pending approvals";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id)
        {
            try
            {
                await _mediator.Send(new ApproveDisasterCommand
                {
                    Id = id,
                    ApprovedBy = User.Identity.Name
                });

                TempData["SuccessMessage"] = "Disaster approved successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving disaster with ID: {Id}", id);
                TempData["ErrorMessage"] = $"Error approving disaster: {ex.Message}";
            }

            return RedirectToAction(nameof(Approvals));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id)
        {
            try
            {
                await _mediator.Send(new RejectDisasterCommand
                {
                    Id = id,
                    RejectedBy = User.Identity.Name
                });

                TempData["SuccessMessage"] = "Disaster rejected successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting disaster with ID: {Id}", id);
                TempData["ErrorMessage"] = $"Error rejecting disaster: {ex.Message}";
            }

            return RedirectToAction(nameof(Approvals));
        }

        [HttpPost]
        public JsonResult GetDisastersJson([FromBody] DisasterListModel model)
        {
            try
            {
                var (data, total, totalDisplay) = _disasterService.GetDisasters(
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
                        title = HttpUtility.HtmlEncode(d.Title),
                        location = HttpUtility.HtmlEncode(d.Location),
                        severity = d.Severity.ToString(),
                        occurredDate = d.OccurredDate.ToString("yyyy-MM-dd"),
                        affectedPeople = d.AffectedPeople.ToString(),
                        status = d.Status.ToString(),
                        actions = GetActionButtons(d)
                    }).ToArray()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching disasters for DataTables");
                return Json(new
                {
                    draw = model.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new object[0]
                });
            }
        }

        private string GetActionButtons(Disaster disaster)
        {
            var buttons = new StringBuilder();

            // Edit button
            if (User.IsInRole("Admin") || (User.IsInRole("Field Representative") && disaster.CreatedBy == User.Identity.Name))
            {
                buttons.Append($"<a href=\"/Admin/Disasters/Edit/{disaster.Id}\" class=\"btn btn-sm btn-outline-primary me-1\">");
                buttons.Append("<i class=\"bi bi-pencil-square\"></i></a>");
            }

            // Delete button
            if (User.IsInRole("Admin") || (User.IsInRole("Field Representative") && disaster.CreatedBy == User.Identity.Name))
            {
                buttons.Append($"<a href=\"/Admin/Disasters/DeleteConfirmation/{disaster.Id}\" class=\"btn btn-sm btn-outline-danger\">");
                buttons.Append("<i class=\"bi bi-trash\"></i></a>");
            }

            return buttons.ToString();
        }

        public IActionResult Edit(Guid id)
        {
            try
            {
                var disaster = _disasterService.GetDisaster(id);
                if (disaster == null)
                {
                    _logger.LogWarning("Disaster not found with ID: {Id}", id);
                    return NotFound();
                }

                // Check authorization - only admin or the creator can edit
                if (!User.IsInRole("Admin") && disaster.CreatedBy != User.Identity.Name)
                {
                    return Forbid();
                }

                return View(new EditDisasterModel
                {
                    Id = disaster.Id,
                    Title = disaster.Title,
                    Description = disaster.Description,
                    Location = disaster.Location,
                    OccurredDate = disaster.OccurredDate,
                    Severity = disaster.Severity,
                    AffectedPeople = disaster.AffectedPeople,
                    RequiredAssistance = disaster.RequiredAssistance
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading disaster for edit with ID: {Id}", id);
                TempData["ErrorMessage"] = "Error loading disaster details";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditDisasterModel model)
        {
            if (!ModelState.IsValid)
            {
                // Return the view with validation errors
                return View(model);
            }

            try
            {
                var existingDisaster = _disasterService.GetDisaster(model.Id);
                if (existingDisaster == null)
                {
                    return NotFound();
                }

                // Check authorization - only admin or the creator can edit
                if (!User.IsInRole("Admin") && existingDisaster.CreatedBy != User.Identity.Name)
                {
                    return Forbid();
                }

                // Update the disaster
                existingDisaster.Title = model.Title;
                existingDisaster.Description = model.Description;
                existingDisaster.Location = model.Location;
                existingDisaster.OccurredDate = model.OccurredDate;
                existingDisaster.Severity = model.Severity;
                existingDisaster.AffectedPeople = model.AffectedPeople;
                existingDisaster.RequiredAssistance = model.RequiredAssistance;

                _disasterService.UpdateDisaster(existingDisaster);

                TempData["SuccessMessage"] = "Disaster updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating disaster with ID: {Id}", model.Id);
                ModelState.AddModelError("", $"Error updating disaster: {ex.Message}");
                return View(model);
            }
        }

        public IActionResult DeleteConfirmation(Guid id)
        {
            try
            {
                var disaster = _disasterService.GetDisaster(id);
                if (disaster == null)
                {
                    _logger.LogWarning("Disaster not found for deletion with ID: {Id}", id);
                    return NotFound();
                }

                // Check authorization - only admin or the creator can delete
                if (!User.IsInRole("Admin") && disaster.CreatedBy != User.Identity.Name)
                {
                    return Forbid();
                }

                return View(new DeleteDisasterModel
                {
                    Id = disaster.Id,
                    Title = disaster.Title
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading disaster for deletion with ID: {Id}", id);
                TempData["ErrorMessage"] = "Error loading disaster for deletion";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id)
        {
            try
            {
                var disaster = _disasterService.GetDisaster(id);
                if (disaster == null)
                {
                    _logger.LogWarning("Disaster not found for deletion with ID: {Id}", id);
                    return NotFound();
                }

                // Check authorization - only admin or the creator can delete
                if (!User.IsInRole("Admin") && disaster.CreatedBy != User.Identity.Name)
                {
                    return Forbid();
                }

                _disasterService.DeleteDisaster(id);

                TempData["SuccessMessage"] = "Disaster deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting disaster with ID: {Id}", id);
                TempData["ErrorMessage"] = $"Error deleting disaster: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}