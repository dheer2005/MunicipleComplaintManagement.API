namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class WorkerDto
    {
        public Guid WorkerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public bool IsActive { get; set; }
        public int CurrentAssignments { get; set; }

    }
}
