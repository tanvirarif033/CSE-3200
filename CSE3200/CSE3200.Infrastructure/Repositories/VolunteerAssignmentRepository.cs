using CSE3200.Domain.Entities;
using CSE3200.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSE3200.Infrastructure.Repositories
{
    public class VolunteerAssignmentRepository : Repository<VolunteerAssignment, Guid>, IVolunteerAssignmentRepository
    {
        public VolunteerAssignmentRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public IList<VolunteerAssignment> GetAssignmentsByDisaster(Guid disasterId)
        {
            return GetDynamic(
                filter: x => x.DisasterId == disasterId && x.Status == "Assigned",
                orderBy: "AssignedDate DESC",
                include: null,
                isTrackingOff: false
            );
        }

        public IList<VolunteerAssignment> GetAssignmentsByVolunteer(string volunteerUserId)
        {
            return GetDynamic(
                filter: x => x.VolunteerUserId == volunteerUserId && x.Status == "Assigned",
                orderBy: "AssignedDate DESC",
                include: null,
                isTrackingOff: false
            );
        }

        public int GetVolunteerCountByDisaster(Guid disasterId)
        {
            return GetCount(x => x.DisasterId == disasterId && x.Status == "Assigned");
        }

        public bool IsVolunteerAssigned(Guid disasterId, string volunteerUserId)
        {
            return GetCount(x => x.DisasterId == disasterId &&
                               x.VolunteerUserId == volunteerUserId &&
                               x.Status == "Assigned") > 0;
        }

        // Add this method if it doesn't exist
        public IList<VolunteerAssignment> GetAll()
        {
            return GetDynamic(
                filter: null,
                orderBy: "AssignedDate DESC",
                include: null,
                isTrackingOff: false
            );
        }
    }
}