using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace CSE3200.Infrastructure.Identity
{
    public class ApplicationUserManager
        : UserManager<ApplicationUser>
    {
        private readonly ApplicationDbContext _context;
        public ApplicationUserManager(
            IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<ApplicationUser>> logger,
            ApplicationDbContext context) // Add context parameter
            : base(store, optionsAccessor, passwordHasher, userValidators,
                  passwordValidators, keyNormalizer, errors, services, logger)
        {
            _context = context;
        }

        // Add helper methods for volunteer management
        public async Task<List<ApplicationUser>> GetUsersByVolunteerStatusAsync(string status)
        {
            return await Users.Where(u => u.VolunteerRequestStatus == status).ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetPendingVolunteerRequestsAsync()
        {
            return await Users
                .Where(u => u.IsVolunteerRequested && u.VolunteerRequestStatus == "Pending")
                .ToListAsync();
        }

        public async Task<int> GetVolunteerCountAsync()
        {
            return await Users.CountAsync(u => u.VolunteerRequestStatus == "Approved");
        }

        public async Task<bool> UpdateUserProfileAsync(ApplicationUser user)
        {
            var result = await UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<List<ApplicationUser>> SearchUsersAsync(string searchTerm)
        {
            return await Users
                .Where(u => u.FirstName.Contains(searchTerm) ||
                           u.LastName.Contains(searchTerm) ||
                           u.Email.Contains(searchTerm) ||
                           u.UserName.Contains(searchTerm) ||
                           (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)))
                .ToListAsync();
        }
    }
}
