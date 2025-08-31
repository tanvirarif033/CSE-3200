namespace CSE3200.Web.Areas.Admin.Models
{
    public class DisasterDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Severity { get; set; }
        public DateTime OccurredDate { get; set; }
        public int AffectedPeople { get; set; }
        public string Status { get; set; }
        public int VolunteerCount { get; set; }
        public List<VolunteerAssignmentViewModel> VolunteerAssignments { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }

}
