// ProfileController.cs
using CSE3200.Application.Services;
using CSE3200.Domain.Services;
using CSE3200.Infrastructure.Identity;
using CSE3200.Web.Models;
//using CSE3200.Web.Models.CSE3200.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSE3200.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImageService _imageService;
        private readonly ILogger<ProfileController> _logger;
        private readonly IVolunteerAssignmentService _volunteerAssignmentService;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IImageService imageService,
            IVolunteerAssignmentService volunteerAssignmentService,
            ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _imageService = imageService;
            _volunteerAssignmentService = volunteerAssignmentService;
            _logger = logger;
        }

        // GET: Profile/Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ProfileViewModel
            {
                Id = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                State = user.State,
                ZipCode = user.ZipCode,
                EmergencyContactName = user.EmergencyContactName,
                EmergencyContactPhone = user.EmergencyContactPhone,
                Skills = user.Skills,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsVolunteerRequested = user.IsVolunteerRequested,
                VolunteerRequestStatus = user.VolunteerRequestStatus,
                RegistrationDate = user.RegistrationDate
            };

            return View(model);
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ProfileViewModel
            {
                Id = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                State = user.State,
                ZipCode = user.ZipCode,
                EmergencyContactName = user.EmergencyContactName,
                EmergencyContactPhone = user.EmergencyContactPhone,
                Skills = user.Skills,
                ProfilePictureUrl = user.ProfilePictureUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            // Remove ProfilePicture validation since it's commented out in the view
            ModelState.Remove("ProfilePicture");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            try
            {
                // Handle profile picture upload only if provided (optional now)
                if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
                {
                    var imagePath = await _imageService.SaveProfileImageAsync(model.ProfilePicture, user.Id.ToString());
                    user.ProfilePictureUrl = imagePath;
                }

                // Update user properties
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;
                user.City = model.City;
                user.State = model.State;
                user.ZipCode = model.ZipCode;
                user.EmergencyContactName = model.EmergencyContactName;
                user.EmergencyContactPhone = model.EmergencyContactPhone;
                user.Skills = model.Skills;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating your profile.");
            }

            return View(model);
        }

        // GET: Profile/RequestVolunteer
        public async Task<IActionResult> RequestVolunteer()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (user.IsVolunteerRequested)
            {
                TempData["InfoMessage"] = "You have already requested to become a volunteer.";
                return RedirectToAction(nameof(Index));
            }

            var model = new VolunteerRequestModel
            {
                UserId = user.Id.ToString(),
                Skills = user.Skills,
                EmergencyContactName = user.EmergencyContactName,
                EmergencyContactPhone = user.EmergencyContactPhone
            };

            return View(model);
        }

        // POST: Profile/RequestVolunteer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestVolunteer(VolunteerRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            try
            {
                // Update user skills and emergency contact info
                user.Skills = model.Skills;
                user.EmergencyContactName = model.EmergencyContactName;
                user.EmergencyContactPhone = model.EmergencyContactPhone;
                user.IsVolunteerRequested = true;
                user.VolunteerRequestStatus = "Pending";

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Volunteer request submitted successfully! Admin will review your application.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting volunteer request for user {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "An error occurred while submitting your volunteer request.");
            }

            return View(model);
        }

        // GET: Profile/VolunteerAssignments
        public async Task<IActionResult> VolunteerAssignments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Check if user is a volunteer
            var isVolunteer = await _userManager.IsInRoleAsync(user, "Volunteer");
            if (!isVolunteer)
            {
                TempData["ErrorMessage"] = "You are not registered as a volunteer.";
                return RedirectToAction(nameof(Index));
            }

            // You'll need to implement this service method to get volunteer assignments
            // Use the existing VolunteerAssignmentService
            var assignments = _volunteerAssignmentService.GetVolunteerAssignments(user.Id.ToString());

            return View( assignments );
        }
    }
}