namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class ComplaintAssignmentHistoryDto
    {
        public Guid ComplaintId { get; set; }
        public string? PreviousWorkerName { get; set; }
        public string NewWorkerName { get; set; } = string.Empty;
        public string AssignedBy { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public string? Reason { get; set; }
    }
}
