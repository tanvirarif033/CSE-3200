using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CSE3200.Infrastructure.Identity
{
    public class ApplicationRoleManager
        : RoleManager<ApplicationRole>
    {
        private readonly ApplicationDbContext _context;
        public ApplicationRoleManager(IRoleStore<ApplicationRole> store,
            IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
            ILogger<RoleManager<ApplicationRole>> logger,
             ApplicationDbContext context)
            : base(store, roleValidators, keyNormalizer, errors, logger)
        {
            _context = context;

        }
        // Add helper methods for role management
        public async Task<List<ApplicationRole>> GetActiveRolesAsync()
        {
            return await Roles.ToListAsync();
        }

        public async Task<ApplicationRole> FindRoleByNameAsync(string roleName)
        {
            return await FindByNameAsync(roleName);
        }

        public async Task<List<string>> GetUserRolesAsync(Guid userId)
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Join(_context.Roles,
                    userRole => userRole.RoleId,
                    role => role.Id,
                    (userRole, role) => role.Name)
                .ToListAsync();

            return userRoles;
        }
    }
}
