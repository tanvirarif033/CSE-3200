using CSE3200.Application.Features.Volunteers.Commands;
using CSE3200.Application.Features.Volunteers.Queries;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using CSE3200.Infrastructure.Identity;
using CSE3200.Web.Areas.Admin.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSE3200.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class VolunteerController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDisasterService _disasterService;
        private readonly IVolunteerAssignmentService _volunteerService;
        private readonly ILogger<VolunteerController> _logger;

        public VolunteerController(
            IMediator mediator,
            UserManager<ApplicationUser> userManager,
            IDisasterService disasterService,
            IVolunteerAssignmentService volunteerService,
            ILogger<VolunteerController> logger)
        {
            _mediator = mediator;
            _userManager = userManager;
            _disasterService = disasterService;
            _volunteerService = volunteerService;
            _logger = logger;
        }

        // GET: All volunteer assignments
        public async Task<IActionResult> Index()
        {
            try
            {
                var assignments = await _mediator.Send(new GetAllVolunteerAssignmentsQuery());

                // Get user details for each assignment
                var assignmentsWithUsers = new List<VolunteerAssignmentViewModel>();
                foreach (var assignment in assignments)
                {
                    var user = await _userManager.FindByIdAsync(assignment.VolunteerUserId);
                    var disaster = _disasterService.GetDisaster(assignment.DisasterId);

                    if (user != null && disaster != null)
                    {
                        assignmentsWithUsers.Add(new VolunteerAssignmentViewModel
                        {
                            Id = assignment.Id,
                            DisasterId = assignment.DisasterId,
                            DisasterTitle = disaster.Title,
                            VolunteerUserId = assignment.VolunteerUserId,
                            VolunteerName = $"{user.FirstName} {user.LastName}",
                            VolunteerEmail = user.Email,
                            TaskDescription = assignment.TaskDescription,
                            AssignedDate = assignment.AssignedDate,
                            AssignedBy = assignment.AssignedBy,
                            Status = assignment.Status
                        });
                    }
                }

                return View(assignmentsWithUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading volunteer assignments");
                TempData["ErrorMessage"] = "Error loading volunteer assignments";
                return View(new List<VolunteerAssignmentViewModel>());
            }
        }

        // GET: Assign volunteer to disaster
        public async Task<IActionResult> Assign(Guid disasterId)
        {
            try
            {
                var disaster = _disasterService.GetDisaster(disasterId);
                if (disaster == null)
                {
                    TempData["ErrorMessage"] = "Disaster not found";
                    return RedirectToAction("Index", "Disasters");
                }

                if (disaster.Status != DisasterStatus.Approved)
                {
                    TempData["ErrorMessage"] = "Cannot assign volunteers to a disaster that is not approved. Current status: " + disaster.Status;
                    return RedirectToAction("Details", "Disasters", new { id = disasterId });
                }

                var volunteers = await GetAvailableVolunteers();

                var model = new AssignVolunteerModel
                {
                    DisasterId = disasterId,
                    DisasterTitle = disaster.Title,
                    AvailableVolunteers = volunteers
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading assign volunteer page for disaster {DisasterId}", disasterId);
                TempData["ErrorMessage"] = "Error loading assignment page";
                return RedirectToAction("Index", "Disasters");
            }
        }

        // POST: Assign volunteer to disaster
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(AssignVolunteerModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableVolunteers = await GetAvailableVolunteers();
                return View(model);
            }

            try
            {
                // Verify disaster still exists and is approved
                var disaster = _disasterService.GetDisaster(model.DisasterId);
                if (disaster == null || disaster.Status != DisasterStatus.Approved)
                {
                    TempData["ErrorMessage"] = "Disaster not found or no longer approved";
                    return RedirectToAction("Index", "Disasters");
                }

                var command = new AssignVolunteerCommand
                {
                    DisasterId = model.DisasterId,
                    VolunteerUserId = model.VolunteerUserId,
                    TaskDescription = model.TaskDescription,
                    AssignedBy = User.Identity.Name
                };

                await _mediator.Send(command);

                TempData["SuccessMessage"] = "Volunteer assigned successfully";
                return RedirectToAction("Volunteers", new { disasterId = model.DisasterId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning volunteer to disaster {DisasterId}", model.DisasterId);
                ModelState.AddModelError("", $"Error assigning volunteer: {ex.Message}");
                model.AvailableVolunteers = await GetAvailableVolunteers();
                return View(model);
            }
        }

        // GET: Volunteers for a specific disaster
        public async Task<IActionResult> Volunteers(Guid disasterId)
        {
            try
            {
                var disaster = _disasterService.GetDisaster(disasterId);
                if (disaster == null)
                {
                    TempData["ErrorMessage"] = "Disaster not found";
                    return RedirectToAction("Index", "Disasters");
                }

                var assignments = await _mediator.Send(new GetDisasterVolunteersQuery { DisasterId = disasterId });

                // Get user details for each assignment
                var assignmentsWithUsers = new List<VolunteerAssignmentViewModel>();
                foreach (var assignment in assignments)
                {
                    var user = await _userManager.FindByIdAsync(assignment.VolunteerUserId);
                    if (user != null)
                    {
                        assignmentsWithUsers.Add(new VolunteerAssignmentViewModel
                        {
                            Id = assignment.Id,
                            DisasterId = assignment.DisasterId,
                            DisasterTitle = disaster.Title,
                            VolunteerUserId = assignment.VolunteerUserId,
                            VolunteerName = $"{user.FirstName} {user.LastName}",
                            VolunteerEmail = user.Email,
                            TaskDescription = assignment.TaskDescription,
                            AssignedDate = assignment.AssignedDate,
                            AssignedBy = assignment.AssignedBy,
                            Status = assignment.Status
                        });
                    }
                }

                var model = new DisasterVolunteersModel
                {
                    DisasterId = disasterId,
                    DisasterTitle = disaster.Title,
                    VolunteerCount = assignments.Count,
                    Assignments = assignmentsWithUsers
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading volunteers for disaster {DisasterId}", disasterId);
                TempData["ErrorMessage"] = "Error loading volunteers";
                return RedirectToAction("Index", "Disasters");
            }
        }

        // GET: Edit volunteer assignment
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var assignment = _volunteerService.GetAssignmentById(id);
                if (assignment == null)
                {
                    TempData["ErrorMessage"] = "Assignment not found";
                    return RedirectToAction(nameof(Index));
                }

                var user = await _userManager.FindByIdAsync(assignment.VolunteerUserId);
                var disaster = _disasterService.GetDisaster(assignment.DisasterId);

                var model = new EditVolunteerAssignmentModel
                {
                    Id = assignment.Id,
                    DisasterId = assignment.DisasterId,
                    DisasterTitle = disaster?.Title ?? "Unknown Disaster",
                    VolunteerUserId = assignment.VolunteerUserId,
                    VolunteerName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown Volunteer",
                    TaskDescription = assignment.TaskDescription,
                    Status = assignment.Status
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading assignment for edit");
                TempData["ErrorMessage"] = "Error loading assignment";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Edit volunteer assignment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditVolunteerAssignmentModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _mediator.Send(new UpdateVolunteerAssignmentCommand
                {
                    Id = model.Id,
                    TaskDescription = model.TaskDescription,
                    Status = model.Status
                });

                if (result)
                {
                    TempData["SuccessMessage"] = "Assignment updated successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error updating assignment";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating assignment");
                ModelState.AddModelError("", $"Error updating assignment: {ex.Message}");
                return View(model);
            }
        }

        // GET: Delete confirmation
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var assignment = _volunteerService.GetAssignmentById(id);
                if (assignment == null)
                {
                    TempData["ErrorMessage"] = "Assignment not found";
                    return RedirectToAction(nameof(Index));
                }

                var user = await _userManager.FindByIdAsync(assignment.VolunteerUserId);
                var disaster = _disasterService.GetDisaster(assignment.DisasterId);

                var model = new DeleteVolunteerAssignmentModel
                {
                    Id = assignment.Id,
                    DisasterTitle = disaster?.Title ?? "Unknown Disaster",
                    VolunteerName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown Volunteer",
                    TaskDescription = assignment.TaskDescription,
                    AssignedDate = assignment.AssignedDate
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading assignment for deletion");
                TempData["ErrorMessage"] = "Error loading assignment";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Delete volunteer assignment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DeleteVolunteerAssignmentModel model)
        {
            try
            {
                var result = await _mediator.Send(new RemoveVolunteerCommand { AssignmentId = model.Id });

                if (result)
                {
                    TempData["SuccessMessage"] = "Assignment deleted successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error deleting assignment";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting assignment");
                TempData["ErrorMessage"] = $"Error deleting assignment: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Remove assignment from disaster volunteers page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAssignment(Guid assignmentId, Guid disasterId)
        {
            try
            {
                var result = await _mediator.Send(new RemoveVolunteerCommand { AssignmentId = assignmentId });

                if (result)
                {
                    TempData["SuccessMessage"] = "Volunteer assignment removed successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error removing assignment";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing assignment");
                TempData["ErrorMessage"] = $"Error removing assignment: {ex.Message}";
            }

            return RedirectToAction("Volunteers", new { disasterId });
        }

        private async Task<List<VolunteerUser>> GetAvailableVolunteers()
        {
            try
            {
                var volunteers = await _userManager.GetUsersInRoleAsync("Volunteer");
                return volunteers.Select(v => new VolunteerUser
                {
                    UserId = v.Id.ToString(),
                    UserName = v.UserName,
                    Email = v.Email,
                    FullName = $"{v.FirstName} {v.LastName}"
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading available volunteers");
                return new List<VolunteerUser>();
            }
        }
    }
}