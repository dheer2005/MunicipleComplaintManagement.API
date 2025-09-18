using System.ComponentModel.DataAnnotations;

namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class BulkAssignDto
    {
        [Required]
        public List<Guid> ComplaintIds { get; set; } = new List<Guid>();

        [Required]
        public Guid WorkerId { get; set; }
    }
}
