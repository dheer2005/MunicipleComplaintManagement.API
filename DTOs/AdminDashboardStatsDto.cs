namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class AdminDashboardStatsDto
    {
        public int TotalCompliants { get; set; }
        public int PendingComplaints { get; set; }
        public int AssignedComplaint { get; set; }
        public int ResolvedComplaints { get; set; }
        public int OverDueComplaint { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalWorkers { get; set; }
        public int ActiveWorkers { get; set; }
        public int TotalOfficials { get; set; }
        public int TotalCitizens { get; set; }
    }
}
