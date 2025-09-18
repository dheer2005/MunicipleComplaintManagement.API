namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class ComplaintCitizenDto
    {
        public Guid ComplaintId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AssignedWorker { get; set; }
        public string Department { get; set; }
    }
}
