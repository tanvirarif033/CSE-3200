using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Repositories;
using CSE3200.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSE3200.Application.Services
{
    public class VolunteerAssignmentService : IVolunteerAssignmentService
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly ILogger<VolunteerAssignmentService> _logger;

        public VolunteerAssignmentService(
            IApplicationUnitOfWork unitOfWork,
            ILogger<VolunteerAssignmentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // ===== Existing methods =====

        public void AssignVolunteer(VolunteerAssignment assignment)
        {
            if (IsVolunteerAlreadyAssigned(assignment.DisasterId, assignment.VolunteerUserId))
                throw new InvalidOperationException("Volunteer is already assigned to this disaster");

            _unitOfWork.VolunteerAssignmentRepository.Add(assignment);
            _unitOfWork.Save();

            _logger.LogInformation("Volunteer {VolunteerId} assigned to disaster {DisasterId} by {AdminId}",
                assignment.VolunteerUserId, assignment.DisasterId, assignment.AssignedBy);
        }

        public void RemoveAssignment(Guid assignmentId)
        {
            var assignment = _unitOfWork.VolunteerAssignmentRepository.GetById(assignmentId);
            if (assignment != null)
            {
                assignment.Status = "Cancelled";
                _unitOfWork.VolunteerAssignmentRepository.Edit(assignment);
                _unitOfWork.Save();
            }
        }

        public IList<VolunteerAssignment> GetDisasterAssignments(Guid disasterId)
        {
            return _unitOfWork.VolunteerAssignmentRepository.GetAssignmentsByDisaster(disasterId);
        }

        public IList<VolunteerAssignment> GetVolunteerAssignments(string volunteerUserId)
        {
            return _unitOfWork.VolunteerAssignmentRepository.GetAssignmentsByVolunteer(volunteerUserId);
        }

        public int GetAssignedVolunteerCount(Guid disasterId)
        {
            return _unitOfWork.VolunteerAssignmentRepository.GetVolunteerCountByDisaster(disasterId);
        }

        public bool IsVolunteerAlreadyAssigned(Guid disasterId, string volunteerUserId)
        {
            return _unitOfWork.VolunteerAssignmentRepository.IsVolunteerAssigned(disasterId, volunteerUserId);
        }

        public VolunteerAssignment GetAssignmentById(Guid assignmentId)
        {
            return _unitOfWork.VolunteerAssignmentRepository.GetById(assignmentId);
        }

        public void UpdateAssignment(VolunteerAssignment assignment)
        {
            _unitOfWork.VolunteerAssignmentRepository.Edit(assignment);
            _unitOfWork.Save();
            _logger.LogInformation("Volunteer assignment {AssignmentId} updated successfully", assignment.Id);
        }

        public IList<VolunteerAssignment> GetAllAssignments()
        {
            return _unitOfWork.VolunteerAssignmentRepository.GetAll();
        }

        // ===== New interface methods =====

        public IList<VolunteerAssignment> GetAvailableVolunteersForDisaster(Guid disasterId)
        {
            // Placeholder – requires User/Volunteer repository to truly implement.
            return new List<VolunteerAssignment>();
        }

        public void CompleteAssignment(Guid assignmentId, string notes, int actualHours)
        {
            var assignment = _unitOfWork.VolunteerAssignmentRepository.GetById(assignmentId);
            if (assignment != null)
            {
                assignment.Status = "Completed";
                assignment.Notes = notes ?? assignment.Notes;
                assignment.ActualHours = actualHours;
                assignment.EndDate = DateTime.UtcNow;

                _unitOfWork.VolunteerAssignmentRepository.Edit(assignment);
                _unitOfWork.Save();

                _logger.LogInformation("Assignment {AssignmentId} marked as completed", assignmentId);
            }
        }

        public void CancelAssignment(Guid assignmentId, string reason)
        {
            var assignment = _unitOfWork.VolunteerAssignmentRepository.GetById(assignmentId);
            if (assignment != null)
            {
                assignment.Status = "Cancelled";
                assignment.Notes = reason ?? assignment.Notes;
                assignment.EndDate = DateTime.UtcNow;

                _unitOfWork.VolunteerAssignmentRepository.Edit(assignment);
                _unitOfWork.Save();

                _logger.LogInformation("Assignment {AssignmentId} cancelled. Reason: {Reason}", assignmentId, reason);
            }
        }

        public IList<VolunteerAssignment> GetAssignmentsByStatus(string status)
        {
            return _unitOfWork.VolunteerAssignmentRepository.GetAssignmentsByStatus(status);
        }

        public IList<VolunteerAssignmentStats> GetVolunteerStats(string volunteerUserId)
        {
            var assignments = _unitOfWork.VolunteerAssignmentRepository.GetAssignmentsByVolunteer(volunteerUserId);
            var completed = assignments.Where(a => a.Status == "Completed").ToList();

            return new List<VolunteerAssignmentStats>
            {
                new VolunteerAssignmentStats
                {
                    VolunteerUserId = volunteerUserId,
                    VolunteerName = string.Empty, // populate from user repo if available
                    TotalAssignments = assignments.Count,
                    CompletedAssignments = completed.Count,
                    TotalHours = completed.Sum(a => a.ActualHours),
                    AverageRating = 0m // placeholder for rating system
                }
            };
        }

        public IList<DisasterVolunteerStats> GetDisasterVolunteerStats(Guid disasterId)
        {
            var assignments = _unitOfWork.VolunteerAssignmentRepository.GetAssignmentsByDisaster(disasterId);

            return new List<DisasterVolunteerStats>
            {
                new DisasterVolunteerStats
                {
                    DisasterId = disasterId,
                    DisasterTitle = string.Empty, // populate from disaster repo if available
                    RequiredVolunteers = 0,       // fill if tracked in Disaster entity
                    AssignedVolunteers = assignments.Count,
                    CompletedAssignments = assignments.Count(a => a.Status == "Completed"),
                    PendingAssignments = assignments.Count(a => a.Status == "Assigned" || a.Status == "InProgress")
                }
            };
        }
    }
}
