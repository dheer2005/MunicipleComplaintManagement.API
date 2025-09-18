namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class FeedbackDto
    {
        public bool IsSatisfied { get; set; }
        public int Rating { get; set; }
        public string? Comments { get; set; }
    }
}
