namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class WorkUpdateDto
    {
        public Guid WorkUpdateId { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public int CompletionPercentage { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? EstimatedCompletionDate { get; set; }
        public bool RequiresAdditionalResources { get; set; }
        public string AdditionalResourcesNeeded { get; set; }
        public List<ComplaintAttachmentDto>? Attachments { get; set; }
    }
}
