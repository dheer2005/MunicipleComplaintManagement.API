using MunicipleComplaintMgmtSys.API.Enums;

namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
    }
}
