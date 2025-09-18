namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class ComplaintFilterDto
    {
        public int? DepartmentId { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public bool? IsOverdue { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
