using MunicipleComplaintMgmtSys.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace MunicipleComplaintMgmtSys.API.Models
{
    public class ComplaintAttachment
    {
        [Key]
        public int AttachmentId { get; set; }
        public Guid ComplaintId { get; set; }
        public Guid UploadedByUserId { get; set; }
        public string? ImageUrl {  get; set; }
        public AttachmentType AttachmentType { get; set; } = AttachmentType.CitizenProof;
        public DateTime UploadedAt { get; set; }
        public Complaint Complaint { get; set; } = null!;
        public User UploadedByUser { get; set; } = null!;
    }
}
