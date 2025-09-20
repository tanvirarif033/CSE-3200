// IVolunteerAssignmentService.cs (in CSE3200.Domain.Services namespace)
using CSE3200.Domain.Entities;
using System;
using System.Collections.Generic;

namespace CSE3200.Domain.Services
{
    public interface IVolunteerAssignmentService
    {
        void AssignVolunteer(VolunteerAssignment assignment);
        void RemoveAssignment(Guid assignmentId);
        IList<VolunteerAssignment> GetDisasterAssignments(Guid disasterId);
        IList<VolunteerAssignment> GetVolunteerAssignments(string volunteerUserId);
        int GetAssignedVolunteerCount(Guid disasterId);
        bool IsVolunteerAlreadyAssigned(Guid disasterId, string volunteerUserId);

        // Existing methods
        VolunteerAssignment GetAssignmentById(Guid assignmentId);
        void UpdateAssignment(VolunteerAssignment assignment);
        IList<VolunteerAssignment> GetAllAssignments();

        // Add these new methods
        IList<VolunteerAssignment> GetAvailableVolunteersForDisaster(Guid disasterId);
        void CompleteAssignment(Guid assignmentId, string notes, int actualHours);
        void CancelAssignment(Guid assignmentId, string reason);
        IList<VolunteerAssignment> GetAssignmentsByStatus(string status);
        IList<VolunteerAssignmentStats> GetVolunteerStats(string volunteerUserId);
        IList<DisasterVolunteerStats> GetDisasterVolunteerStats(Guid disasterId);
    }

    // Supporting classes
    public class VolunteerAssignmentStats
    {
        public string VolunteerUserId { get; set; }
        public string VolunteerName { get; set; }
        public int TotalAssignments { get; set; }
        public int CompletedAssignments { get; set; }
        public int TotalHours { get; set; }
        public decimal AverageRating { get; set; }
    }

    public class DisasterVolunteerStats
    {
        public Guid DisasterId { get; set; }
        public string DisasterTitle { get; set; }
        public int RequiredVolunteers { get; set; }
        public int AssignedVolunteers { get; set; }
        public int CompletedAssignments { get; set; }
        public int PendingAssignments { get; set; }
    }
}