namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class ComplaintAdminDto
    {
        public Guid ComplaintId { get; set; }
        public string Title { get; set; }
        public string CitizenName { get; set; }
        public string Department { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
