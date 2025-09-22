using CSE3200.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Application.Features.Users.Commands
{
    // UpdateProfileCommandHandler.cs
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateProfileCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null) return;

            user.PhoneNumber = request.PhoneNumber;
            user.Address = request.Address;
            user.City = request.City;
            user.State = request.State;
            user.ZipCode = request.ZipCode;
            user.EmergencyContactName = request.EmergencyContactName;
            user.EmergencyContactPhone = request.EmergencyContactPhone;
            user.Skills = request.Skills;
            user.ProfilePictureUrl = request.ProfilePictureUrl;

            await _userManager.UpdateAsync(user);
        }
    }

    // RequestVolunteerCommandHandler.cs
    public class RequestVolunteerCommandHandler : IRequestHandler<RequestVolunteerCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RequestVolunteerCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task Handle(RequestVolunteerCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null) return;

            user.IsVolunteerRequested = true;
            user.VolunteerRequestDate = DateTime.UtcNow;
            user.VolunteerRequestStatus = "Pending";

            await _userManager.UpdateAsync(user);
        }
    }

    // UpdateVolunteerStatusCommandHandler.cs
    public class UpdateVolunteerStatusCommandHandler : IRequestHandler<UpdateVolunteerStatusCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateVolunteerStatusCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task Handle(UpdateVolunteerStatusCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null) return;

            user.VolunteerRequestStatus = request.Status;

            if (request.Status == "Approved")
            {
                // Remove from current roles and add to Volunteer role
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, "Volunteer");
            }

            await _userManager.UpdateAsync(user);
        }
    }
}
