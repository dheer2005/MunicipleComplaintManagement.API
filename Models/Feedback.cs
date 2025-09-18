namespace MunicipleComplaintMgmtSys.API.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public Guid ComplaintId { get; set; }
        public Guid CitizenId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsSatisfied { get; set; }
        public DateTime CreatedAt { get; set; }

        public Complaint Complaint { get; set; } = null!;
        public User Citizen { get; set; } = null!;
    }
}
