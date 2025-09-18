namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class ComplaintWorkerDto
    {
        public Guid ComplaintId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CitizenName { get; set; }
        public string CitizenAddress { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
