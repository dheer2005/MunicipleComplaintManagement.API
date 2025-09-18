namespace MunicipleComplaintMgmtSys.API.Models
{
    public class Worker
    {
        public Guid WorkerId { get; set; }
        public Guid UserId { get; set; }
        public int DepartmentId { get; set; }
        public bool IsAvailable { get; set; }

        public User User { get; set; } = null!;
        public Department Department { get; set; } = null!;
        public ICollection<Complaint> AssignedComplaints { get; set; } = new List<Complaint>();
    }
}
