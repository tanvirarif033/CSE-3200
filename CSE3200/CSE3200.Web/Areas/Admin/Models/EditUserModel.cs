using System;
using System.ComponentModel.DataAnnotations;

namespace CSE3200.Web.Areas.Admin.Models
{
    public class EditUserModel
    {
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
    }
}