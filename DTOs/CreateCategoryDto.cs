namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class CreateCategoryDto
    {
        public int DepartmentId { get; set; }
        public string CategoryName { get; set; }
        public int DefaultSlaHours {  get; set; }
    }
}
