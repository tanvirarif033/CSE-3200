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

        // New methods added
        VolunteerAssignment GetAssignmentById(Guid assignmentId);
        void UpdateAssignment(VolunteerAssignment assignment);
        IList<VolunteerAssignment> GetAllAssignments();
    }
}