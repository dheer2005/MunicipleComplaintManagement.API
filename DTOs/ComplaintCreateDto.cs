using MunicipleComplaintMgmtSys.API.Enums;

namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class ComplaintCreateDto
    {
        public Guid CitizenId { get; set; }
        public string? Description { get; set; }
        public int DepartmentId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? AddressText { get; set; }
        public ComplaintStatus CurrentStatus { get; set; } = ComplaintStatus.Pending;
        public List<IFormFile>? Attachments { get; set; }
        public AttachmentType AttachmentType { get; set; } = AttachmentType.CitizenProof;

    }
}
