namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class WorkerAssignmentDto
    {
        public int WorkerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int CurrentAssignments { get; set; }
        public int MaxCapacity { get; set; } = 10; // Default capacity
        public bool IsAvailable => CurrentAssignments < MaxCapacity;
        public List<ComplaintSummaryDto> AssignedComplaints { get; set; } = new List<ComplaintSummaryDto>();
    }
}
