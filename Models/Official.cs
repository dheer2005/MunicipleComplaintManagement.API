namespace MunicipleComplaintMgmtSys.API.Models
{
    public class Official
    {
        public Guid OfficialId { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
