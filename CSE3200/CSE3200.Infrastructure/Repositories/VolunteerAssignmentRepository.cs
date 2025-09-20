using CSE3200.Domain.Entities;
using CSE3200.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSE3200.Infrastructure.Repositories
{
    public class VolunteerAssignmentRepository : Repository<VolunteerAssignment, Guid>, IVolunteerAssignmentRepository
    {
        private readonly ApplicationDbContext _context;

        public VolunteerAssignmentRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public IList<VolunteerAssignment> GetAssignmentsByDisaster(Guid disasterId)
        {
            return GetDynamic(
                filter: x => x.DisasterId == disasterId && x.Status == "Assigned",
                orderBy: "AssignedDate DESC", // Use string instead of lambda
                include: null,
                isTrackingOff: false
            );
        }

        public IList<VolunteerAssignment> GetAssignmentsByVolunteer(string volunteerUserId)
        {
            return GetDynamic(
                filter: x => x.VolunteerUserId == volunteerUserId && x.Status == "Assigned",
                orderBy: "AssignedDate DESC", // Use string instead of lambda
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

        public IList<VolunteerAssignment> GetAll()
        {
            return GetDynamic(
                filter: null,
                orderBy: "AssignedDate DESC", // Use string instead of lambda
                include: null,
                isTrackingOff: false
            );
        }



        // Implement the new interface methods
        public IList<VolunteerAssignment> GetAssignmentsByStatus(string status)
        {
            return GetDynamic(
                filter: x => x.Status == status,
                orderBy: "AssignedDate DESC", // Use string instead of lambda
                include: null,
                isTrackingOff: false
            );
        }

        public IList<VolunteerAssignment> GetActiveAssignments()
        {
            return GetDynamic(
                filter: x => x.Status == "Assigned" || x.Status == "InProgress",
                orderBy: "AssignedDate DESC", // Use string instead of lambda
                include: null,
                isTrackingOff: false
            );
        }

        public IList<VolunteerAssignment> GetAssignmentsByDateRange(DateTime startDate, DateTime endDate)
        {
            return GetDynamic(
                filter: x => x.AssignedDate >= startDate && x.AssignedDate <= endDate,
                orderBy: "AssignedDate DESC", // Use string instead of lambda
                include: null,
                isTrackingOff: false
            );
        }

        public IList<VolunteerAssignment> GetAssignmentsWithUsers(Guid disasterId)
        {
            // Since we removed the navigation property, return basic assignments
            // You can implement a join query in a service layer if needed
            return GetDynamic(
                filter: x => x.DisasterId == disasterId,
                orderBy: "AssignedDate DESC", // Use string instead of lambda
                include: null,
                isTrackingOff: false
            );
        }

        public int GetCompletedAssignmentCount(string volunteerUserId)
        {
            return GetCount(x => x.VolunteerUserId == volunteerUserId && x.Status == "Completed");
        }

        // Alternative method if you need user information
        public IList<VolunteerAssignmentWithUserInfo> GetAssignmentsWithUserInfo(Guid disasterId)
        {
            // This would require a join query since we don't have navigation properties
            var assignmentsWithUsers = (from assignment in _context.VolunteerAssignments
                                        join user in _context.Users on assignment.VolunteerUserId equals user.Id.ToString()
                                        where assignment.DisasterId == disasterId
                                        select new VolunteerAssignmentWithUserInfo
                                        {
                                            Assignment = assignment,
                                            VolunteerName = $"{user.FirstName} {user.LastName}",
                                            VolunteerEmail = user.Email
                                        }).ToList();

            return assignmentsWithUsers;
        }

    }


    // Helper class for assignments with user info
    public class VolunteerAssignmentWithUserInfo
    {
        public VolunteerAssignment Assignment { get; set; }
        public string VolunteerName { get; set; }
        public string VolunteerEmail { get; set; }
    }
}