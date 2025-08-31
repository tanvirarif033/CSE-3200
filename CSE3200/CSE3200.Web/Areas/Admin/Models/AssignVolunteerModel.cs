using System;
using System.Collections.Generic;

namespace CSE3200.Web.Areas.Admin.Models
{
    public class AssignVolunteerModel
    {
        public Guid DisasterId { get; set; }
        public string DisasterTitle { get; set; } = string.Empty;
        public string VolunteerUserId { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public List<VolunteerUser> AvailableVolunteers { get; set; } = new List<VolunteerUser>();
    }

    public class VolunteerUser
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }








}