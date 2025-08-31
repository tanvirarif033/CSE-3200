namespace CSE3200.Web.Areas.Admin.Models
{
    public class DeleteVolunteerAssignmentModel
    {
        public Guid Id { get; set; }
        public string DisasterTitle { get; set; } = string.Empty;
        public string VolunteerName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
    }
}
