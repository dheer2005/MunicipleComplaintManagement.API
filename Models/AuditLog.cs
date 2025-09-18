using System.ComponentModel.DataAnnotations;

namespace MunicipleComplaintMgmtSys.API.Models
{
    public class AuditLog
    {
        [Key]
        public int LogId { get; set; }
        public Guid? UserId { get; set; }
        public string Action { get; set; } = null!;
        public string ActionResult { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User? User { get; set; }

    }
}
