using CSE3200.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Infrastructure.Seeds
{
    public static class ClaimSeed
    {
        public static ApplicationUserClaim[] GetClaims()
        {
            return [
                new ApplicationUserClaim
                {
                    Id = -1,
                    UserId = new Guid("29671297-6bd9-476a-f172-08ddddda291f"),
                    ClaimType = "create_user",
                    ClaimValue = "allowed"
                }
                //new ApplicationUserClaim
                //{
                //    Id = -2, // Ensure this ID is unique
                //    UserId = new Guid("2520631e-ffb2-4f2b-f512-08ddbeddc59f"),
                //    ClaimType = "create_customer",
                //    ClaimValue = "allowed"
                //}
            ];
        }
    }
}
