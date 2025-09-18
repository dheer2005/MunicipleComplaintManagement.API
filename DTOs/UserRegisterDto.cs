using MunicipleComplaintMgmtSys.API.Enums;

namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class UserRegisterDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; } = UserRole.Citizen;
        public string Phone { get; set; }
        public int? DepartmentId { get; set; }
    }
}
