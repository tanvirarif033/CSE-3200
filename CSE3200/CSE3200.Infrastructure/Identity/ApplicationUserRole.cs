using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CSE3200.Infrastructure.Identity
{
    public class ApplicationUserRole
        : IdentityUserRole<Guid>
    {

    }
}
