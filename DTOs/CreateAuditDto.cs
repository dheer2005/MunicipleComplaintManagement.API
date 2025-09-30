namespace MunicipleComplaintMgmtSys.API.DTOs
{
    public class CreateAuditDto
    {
        public Guid? UserId { get; set; }
        public string? Action { get; set; }
        public string? ActionResult { get; set; }
    }
}
