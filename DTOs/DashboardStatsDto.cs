namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalComplaints { get; set; }
        public int PendingComplaints { get; set; }
        public int AssignedComplaints { get; set; }
        public int ResolvedComplaints { get; set; }
        public int OverdueComplaints { get; set; }
        public int TotalDepartments { get; set; }
        public int ActiveWorkers { get; set; }
    }
}
