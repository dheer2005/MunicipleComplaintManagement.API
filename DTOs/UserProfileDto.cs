using MunicipleComplaintMgmtSys.API.Enums;

namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class UserProfileDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // Worker Profile
        public bool? IsWorkerAvailable { get; set; }
        public int? WorkerDepartmentId { get; set; }
        public string? WorkerDepartmentName { get; set; }

        // Official Profile
        public int? OfficialDepartmentId { get; set; }
        public string? OfficialDepartmentName { get; set; }

        // Complaints Summary
        public int TotalComplaints { get; set; }
        public int PendingComplaints { get; set; }
        public int ResolvedComplaints { get; set; }
        public int ClosedComplaints { get; set; }

        // Feedback Summary
        public int TotalFeedbacks { get; set; }
        public double? AverageRating { get; set; }
    }
}
