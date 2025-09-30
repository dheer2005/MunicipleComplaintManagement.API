using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MunicipleComplaintMgmtSys.API.ComplaintContext;
using MunicipleComplaintMgmtSys.API.DTOs;
using MunicipleComplaintMgmtSys.API.Models;

namespace MunicipleComplaintMgmtSys.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : Controller
    {
        private readonly ComplaintDBContext _dbContext;
        public AuditController(ComplaintDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAudit([FromBody] CreateAuditDto dto)
        {

            var audit = new AuditLog
            {
                UserId = dto.UserId,
                Action = dto.Action,
                ActionResult = dto.ActionResult
            };

            _dbContext.AuditLogs.Add(audit);
            await _dbContext.SaveChangesAsync();

            return Ok(audit);
        }

        [HttpGet("GetAllAuditlogs")]
        public async Task<IActionResult> GetAllAuditlogs()
        {
            var auditLogs = await _dbContext.AuditLogs.OrderByDescending(c=>c.CreatedAt).ToListAsync();

            if (auditLogs == null) return NotFound(new { message = "Logs not found" });

            return Ok(auditLogs);
        }
    }
}
