using MunicipleComplaintMgmtSys.API.Enums;

namespace MunicipleComplaintMgmtSys.API.Models
{
    public class Complaint
    {
        public Guid ComplaintId { get; set; }
        public string TicketNo { get; set; } = null!;
        public Guid CitizenId { get; set; }
        public int CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? DepartmentId { get; set; }
        public Guid? AssignedWorkerId { get; set; }
        public bool IsReopened { get; set; }
        public string? Description { get; set; } = null!;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? AddressText { get; set; }
        public ComplaintStatus CurrentStatus { get; set; }
        public DateTime? SlaDueAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User Citizen { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public SubCategory? SubCategory { get; set; }
        public Department? Department { get; set; }
        public Worker? AssignedWorker { get; set; }

        public ICollection<ComplaintAttachment> Attachments { get; set; } = new List<ComplaintAttachment>();
        public ICollection<WorkUpdate> WorkUpdates { get; set; } = new List<WorkUpdate>();
        public Feedback? Feedback { get; set; }
    }
}
