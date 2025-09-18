using System.ComponentModel.DataAnnotations;

namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class ReassignComplaintDto
    {
        [Required]
        public Guid NewWorkerId { get; set; }

        public string? Reason { get; set; }
    }
}
