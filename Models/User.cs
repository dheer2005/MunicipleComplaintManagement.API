using MunicipleComplaintMgmtSys.API.Enums;

namespace MunicipleComplaintMgmtSys.API.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public Worker? WorkerProfile { get; set; }
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}
