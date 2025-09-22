using CSE3200.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Domain.Repositories
{
    // IVolunteerAssignmentRepository.cs
    public interface IVolunteerAssignmentRepository : IRepository<VolunteerAssignment, Guid>
    {
        IList<VolunteerAssignment> GetAssignmentsByDisaster(Guid disasterId);
        IList<VolunteerAssignment> GetAssignmentsByVolunteer(string volunteerUserId);
        int GetVolunteerCountByDisaster(Guid disasterId);
        bool IsVolunteerAssigned(Guid disasterId, string volunteerUserId);

        // Add these new methods
        IList<VolunteerAssignment> GetAssignmentsByStatus(string status);
        IList<VolunteerAssignment> GetActiveAssignments();
        IList<VolunteerAssignment> GetAssignmentsByDateRange(DateTime startDate, DateTime endDate);
        IList<VolunteerAssignment> GetAssignmentsWithUsers(Guid disasterId);
        int GetCompletedAssignmentCount(string volunteerUserId);
    }
}
