using CSE3200.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Infrastructure.Seeds
{
    public static class RoleSeed
    {
        public static ApplicationRole[] GetRoles()
        {
            return [
                new ApplicationRole
                {
                    Id = new Guid("1D348B36-86E9-4E74-9C10-7FB59D035468"),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = new DateTime(2025, 8, 18, 1, 2, 1).ToString(),
                },
                new ApplicationRole
                {
                    Id = new Guid("AE3E5918-2742-4BAC-95FF-2326C1E5966E"),
                    Name = "Donor",
                    NormalizedName = "DONOR",
                    ConcurrencyStamp = new DateTime(2025, 8, 18, 1, 2, 3).ToString(),
                },
                new ApplicationRole
                {
                    Id = new Guid("02352CDF-E55B-458D-91F0-6063EC9300DF"),
                    Name = "Volunteer",
                    NormalizedName = "VOLUNTEER",
                    ConcurrencyStamp = new DateTime(2025, 8, 18, 1, 2, 4).ToString(),
                },
                 new ApplicationRole
                {
                    Id = new Guid("D716BB55-E6B5-4639-A396-2D8C81F30E32"),
                    Name = "Field Representative",
                    NormalizedName = "FIELD REPRESENTATIVE",
                    ConcurrencyStamp = new DateTime(2025, 8, 18, 1, 2, 4).ToString(),
                }
            ];
        }
    }
}
