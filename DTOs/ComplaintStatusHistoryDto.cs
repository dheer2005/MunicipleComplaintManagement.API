namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class ComplaintStatusHistoryDto
    {
        public Guid ComplaintId { get; set; }
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string? Notes { get; set; }
    }
}
