using CSE3200.Application.Features.FAQs.Commands;
using CSE3200.Application.Features.FAQs.Queries;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Services;
using CSE3200.Web.Areas.Admin.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations; // ADD THIS LINE

namespace CSE3200.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FAQController : Controller
    {
        private readonly IFAQService _faqService;
        private readonly IMediator _mediator;
        private readonly ILogger<FAQController> _logger;

        public FAQController(
            IFAQService faqService,
            IMediator mediator,
            ILogger<FAQController> logger)
        {
            _faqService = faqService;
            _mediator = mediator;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View(new AddFAQModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddFAQModel model)
        {
            if (!ModelState.IsValid)
            {
                // ADD MODEL STATE LOGGING
                _logger.LogWarning("ModelState invalid: {@ModelState}", ModelState);
                // END OF ADDED SECTION
                return View(model);
            }

            try
            {
                // ADD REQUEST LOGGING
                _logger.LogInformation("Creating FAQ with model: {@Model}", model);
                // END OF ADDED SECTION

                var command = new AddFAQCommand
                {
                    Question = model.Question,
                    Answer = model.Answer,
                    Category = model.Category,
                    DisplayOrder = model.DisplayOrder,
                    CreatedBy = User.Identity.Name
                };

                // ADD COMMAND LOGGING
                _logger.LogInformation("Command created: {@Command}", command);
                // END OF ADDED SECTION

                var faqId = await _mediator.Send(command);

                TempData["SuccessMessage"] = "FAQ created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating FAQ");

                // ADD DETAILED ERROR LOGGING
                _logger.LogError("User: {User}", User.Identity.Name);
                _logger.LogError("Model: {@Model}", model);
                // END OF ADDED SECTION

                ModelState.AddModelError("", $"Error creating FAQ: {ex.Message}");

                // ADD INNER EXCEPTION DETAILS FOR DEBUGGING
#if DEBUG
                if (ex.InnerException != null)
                {
                    ModelState.AddModelError("", $"Inner exception: {ex.InnerException.Message}");
                }
#endif
                // END OF ADDED SECTION

                return View(model);
            }
        }

        public IActionResult Edit(Guid id)
        {
            var faq = _faqService.GetFAQ(id);
            if (faq == null)
            {
                return NotFound();
            }

            return View(new EditFAQModel
            {
                Id = faq.Id,
                Question = faq.Question,
                Answer = faq.Answer,
                Category = faq.Category,
                DisplayOrder = faq.DisplayOrder
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditFAQModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var command = new UpdateFAQCommand
                {
                    Id = model.Id,
                    Question = model.Question,
                    Answer = model.Answer,
                    Category = model.Category,
                    DisplayOrder = model.DisplayOrder,
                    ModifiedBy = User.Identity.Name
                };

                var success = await _mediator.Send(command);

                if (success)
                {
                    TempData["SuccessMessage"] = "FAQ updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "FAQ not found");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FAQ");
                ModelState.AddModelError("", $"Error updating FAQ: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var command = new ToggleFAQStatusCommand
                {
                    Id = id,
                    ModifiedBy = User.Identity.Name
                };

                var success = await _mediator.Send(command);

                if (success)
                {
                    TempData["SuccessMessage"] = "FAQ status updated successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update FAQ status";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling FAQ status");
                TempData["ErrorMessage"] = $"Error updating FAQ status: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _faqService.DeleteFAQ(id);
                TempData["SuccessMessage"] = "FAQ deleted successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FAQ");
                TempData["ErrorMessage"] = $"Error deleting FAQ: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public JsonResult GetFAQsJson([FromBody] FAQListModel model)
        {
            try
            {
                var faqs = _faqService.GetAllFAQsWithPaging(
                    model.PageIndex,
                    model.PageSize,
                    out int totalCount);

                return Json(new
                {
                    draw = model.Draw,
                    recordsTotal = totalCount,
                    recordsFiltered = totalCount,
                    data = faqs.Select(f => new
                    {
                        id = f.Id.ToString(),
                        question = f.Question,
                        category = f.Category.ToString(),
                        displayOrder = f.DisplayOrder,
                        isActive = f.IsActive,
                        createdDate = f.CreatedDate.ToString("yyyy-MM-dd"),
                        actions = GetActionButtons(f)
                    }).ToArray()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching FAQs for DataTables");
                return Json(new
                {
                    draw = model.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new object[0]
                });
            }
        }

        [HttpGet]
        public IActionResult Test()
        {
            try
            {
                var testFaq = new FAQ
                {
                    Id = Guid.NewGuid(),
                    Question = "Test Question",
                    Answer = "Test Answer",
                    Category = FAQCategory.General,
                    DisplayOrder = 0,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = User.Identity.Name
                };

                _faqService.AddFAQ(testFaq);
                return Ok("Test FAQ created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        private string GetActionButtons(FAQ faq)
        {
            var buttons = new System.Text.StringBuilder();

            // Edit button
            buttons.Append($"<a href=\"/Admin/FAQ/Edit/{faq.Id}\" class=\"btn btn-sm btn-outline-primary me-1\">");
            buttons.Append("<i class=\"bi bi-pencil-square\"></i> Edit</a>");

            // Toggle status button
            var statusText = faq.IsActive ? "Deactivate" : "Activate";
            var statusClass = faq.IsActive ? "warning" : "success";
            buttons.Append($"<button onclick=\"toggleFAQStatus('{faq.Id}')\" class=\"btn btn-sm btn-outline-{statusClass} me-1\">");
            buttons.Append($"<i class=\"bi bi-power\"></i> {statusText}</button>");

            // Delete button
            buttons.Append($"<button onclick=\"confirmDelete('{faq.Id}')\" class=\"btn btn-sm btn-outline-danger\">");
            buttons.Append("<i class=\"bi bi-trash\"></i> Delete</button>");

            return buttons.ToString();
        }
    }
}