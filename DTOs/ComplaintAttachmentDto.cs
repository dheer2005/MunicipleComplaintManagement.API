namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class ComplaintAttachmentDto
    {
        public int AttachmentId { get; set; }
        public string? ImageUrl { get; set; }
        public string AttachmentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}
