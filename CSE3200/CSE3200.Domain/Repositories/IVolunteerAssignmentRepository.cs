using CSE3200.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE3200.Domain.Repositories
{
    public interface IVolunteerAssignmentRepository : IRepository<VolunteerAssignment, Guid>
    {
        IList<VolunteerAssignment> GetAssignmentsByDisaster(Guid disasterId);
        IList<VolunteerAssignment> GetAssignmentsByVolunteer(string volunteerUserId);
        int GetVolunteerCountByDisaster(Guid disasterId);
        bool IsVolunteerAssigned(Guid disasterId, string volunteerUserId);
    }
}
