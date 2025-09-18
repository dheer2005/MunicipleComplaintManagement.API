namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class UserResponseDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
    }
}
