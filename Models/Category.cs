namespace MunicipleComplaintMgmtSys.API.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public int DepartmentId { get; set; }

        public string CategoryName { get; set; } = null!;
        public int DefaultSlaHours { get; set; }
        public Department? Department { get; set; }
        public ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}
