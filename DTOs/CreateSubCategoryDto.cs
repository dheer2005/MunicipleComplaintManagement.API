using MunicipleComplaintMgmtSys.API.Models;

namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class CreateSubCategoryDto
    {
        public int CategoryId { get; set; }
        public string? SubCategoryName { get; set; }
        public int SlaHours { get; set; } = 12; 
    }
}
