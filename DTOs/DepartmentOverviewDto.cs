namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class DepartmentOverviewDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalComplaints { get; set; }
        public int PendingComplaints { get; set; }
        public int AssignedComplaints { get; set; }
        public int ResolvedComplaints { get; set; }
        public int OverdueComplaints { get; set; }
    }
}
