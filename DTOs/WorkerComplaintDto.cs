namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class WorkerComplaintDto
    {
        public Guid ComplaintId { get; set; }
        public string TicketNo { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CurrentStatus { get; set; } = string.Empty;
        public bool IsReopened { get; set; } = false;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? SlaDueAt { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? AddressText { get; set; }

        public string CitizenName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string? SubCategoryName { get; set; }
        public string? DepartmentName { get; set; }
        public string? AssignedWorkerName { get; set; }

        public List<ComplaintAttachmentDto> Attachments { get; set; } = new();
        public FeedbackDto? Feedback { get; set; }
        public List<WorkerUpdateDetailDto> WorkUpdates { get; set; } = new();
    }
}
