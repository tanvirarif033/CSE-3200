using CSE3200.Infrastructure.Identity;
using CSE3200.Application.Features.Users.Commands;
using CSE3200.Application.Features.Users.Queries;
using CSE3200.Domain;
using CSE3200.Infrastructure.Identity;
using CSE3200.Web.Areas.Admin.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CSE3200.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Policy = "UserAddPermission")]
  //  [Area("Admin"), Authorize(Roles="Admin")]
    //[Area("Admin")]
    public class UsersController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsersController(
            IMediator mediator,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _mediator = mediator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Role(string role)
        {
            ViewBag.Role = role;
            return View("Index");
        }

        public IActionResult AddUser()
        {
            return View(new AddUserModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(AddUserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = new AddUserCommand
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Password = model.Password,
                        Role = model.Role,
                        DateOfBirth = model.DateOfBirth
                    };

                    await _mediator.Send(command);

                    TempData["SuccessMessage"] = "User added successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error adding user: {ex.Message}");
                    TempData["ErrorMessage"] = "Failed to create user.";
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            return View(new EditUserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Role = role
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var command = new UpdateUserCommand
                    {
                        Id = model.Id,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        DateOfBirth = model.DateOfBirth,
                        Role = model.Role
                    };

                    await _mediator.Send(command);

                    TempData["SuccessMessage"] = "User updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating user: {ex.Message}");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
            if (user == null) return NotFound();

            return View(new DeleteUserModel
            {
                Id = user.Id,
                Name = $"{user.FirstName} {user.LastName}"
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DeleteUserModel model)
        {
            try
            {
                await _mediator.Send(new DeleteUserCommand { Id = model.Id });
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting user: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<JsonResult> GetUsersJson([FromBody] UserListModel model)
        {
            try
            {
                var query = new GetUsersListQuery
                {
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    Order = model.GetSortExpression(),
                    Search = model.Search,
                    RoleFilter = model.RoleFilter
                };

                var (data, total, totalDisplay) = await _mediator.Send(query);

                // Create user data array
                var userData = data.Select(u => new
                {
                    id = u.Id.ToString(),
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    email = u.Email,
                    userName = u.UserName,
                    dateOfBirth = u.DateOfBirth.ToString("yyyy-MM-dd"),
                    registrationDate = u.RegistrationDate.ToString("yyyy-MM-dd"),
                    roles = string.Join(", ", _userManager.GetRolesAsync(u).Result)
                }).ToArray();

                return Json(new
                {
                    draw = model.Draw,
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = userData
                });
            }
            catch (Exception ex)
            {
                // Log error
                return Json(new
                {
                    draw = model.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new object[0]
                });
            }
        }
    }
}