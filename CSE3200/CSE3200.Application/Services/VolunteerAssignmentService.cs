using CSE3200.Domain;
using CSE3200.Domain.Entities;
using CSE3200.Domain.Repositories;
using CSE3200.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

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

        public void AssignVolunteer(VolunteerAssignment assignment)
        {
            if (IsVolunteerAlreadyAssigned(assignment.DisasterId, assignment.VolunteerUserId))
            {
                throw new InvalidOperationException("Volunteer is already assigned to this disaster");
            }

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
    }
}