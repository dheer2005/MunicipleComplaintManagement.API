namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class ComplaintSummaryDto
    {
        public Guid ComplaintId { get; set; }
        public string TicketNo { get; set; } = string.Empty;
        public string CitizenName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SubCategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? SlaDueAt { get; set; }
        public bool IsOverdue { get; set; }
        public string? AssignedWorkerName { get; set; }
    }
}
