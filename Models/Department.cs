namespace MunicipleComplaintMgmtSys.API.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public ICollection<Worker> Workers { get; set; } = new List<Worker>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}
