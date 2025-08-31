namespace CSE3200.Web.Areas.Admin.Models
{
    public class EditVolunteerAssignmentModel
    {
        public Guid Id { get; set; }
        public Guid DisasterId { get; set; }
        public string DisasterTitle { get; set; } = string.Empty;
        public string VolunteerUserId { get; set; } = string.Empty;
        public string VolunteerName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public string Status { get; set; } = "Assigned";
    }
}
