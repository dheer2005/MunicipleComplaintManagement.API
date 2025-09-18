namespace MunicipleComplaintMgmtSys.API.Models
{
    public class WorkUpdate
    {
        public Guid WorkUpdateId { get; set; }
        public Guid ComplaintId { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public string Status { get; set; } 
        public string Notes { get; set; }
        public int CompletionPercentage { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? EstimatedCompletionDate { get; set; }
        public bool RequiresAdditionalResources { get; set; }
        public string AdditionalResourcesNeeded { get; set; }

        public Complaint Complaint { get; set; }
        public User UpdatedByUser { get; set; }
        public ICollection<ComplaintAttachment> Attachments { get; set; }

    }
}
