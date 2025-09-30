namespace MunicipleComplaintMgmtSys.API.Models
{
    public class OfficialDepartment
    {
        public Guid OfficialId { get; set; }
        public Official Official { get; set; } = null!;

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
    }
}
