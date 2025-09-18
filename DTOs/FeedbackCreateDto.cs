namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class FeedbackCreateDto
    {
        public Guid ComplaintId { get; set; }
        public Guid CitizenId { get; set; }
        public bool IsSatisfied {  get; set; }
        public int Rating { get; set; }  // 1–5
        public string? Comments { get; set; }
    }
}
