namespace MunicipleComplaintMgmtSys.API.Models
{
    public class SubCategory
    {
        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; }
        public string SubCategoryName { get; set; } = null!;
        public int SlaHours { get; set; }

        public Category Category { get; set; } = null!;
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}
