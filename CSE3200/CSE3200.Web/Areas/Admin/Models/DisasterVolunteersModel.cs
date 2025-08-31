using CSE3200.Domain.Entities;
using System;
using System.Collections.Generic;

namespace CSE3200.Web.Areas.Admin.Models
{
    public class DisasterVolunteersModel
    {
        public Guid DisasterId { get; set; }
        public string DisasterTitle { get; set; } = string.Empty;
        public int VolunteerCount { get; set; }
        public List<VolunteerAssignmentViewModel> Assignments { get; set; } = new List<VolunteerAssignmentViewModel>();
    }
}