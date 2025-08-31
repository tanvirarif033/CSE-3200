namespace CSE3200.Web.Areas.Admin.Models
{
    public class VolunteerAssignmentViewModel
    {
        public Guid Id { get; set; }
        public Guid DisasterId { get; set; }
        public string DisasterTitle { get; set; } = string.Empty;
        public string VolunteerUserId { get; set; } = string.Empty;
        public string VolunteerName { get; set; } = string.Empty;
        public string VolunteerEmail { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
        public string AssignedBy { get; set; } = string.Empty;
        public string Status { get; set; } = "Assigned";
    }
}
