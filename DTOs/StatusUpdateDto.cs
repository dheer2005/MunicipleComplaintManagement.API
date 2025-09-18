using System.ComponentModel.DataAnnotations;

namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class StatusUpdateDto
    {
        public Guid UpdatedByUserId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public string? Notes { get; set; }

        [Range(0, 100)]
        public int CompletionPercentage { get; set; }

        public DateTime? EstimatedCompletionDate { get; set; }

        public bool RequiresAdditionalResources { get; set; }

        public string? AdditionalResourcesNeeded { get; set; }

        public List<IFormFile>? Attachments { get; set; }
    }
}
