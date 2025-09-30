using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MunicipleComplaintMgmtSys.API.ComplaintContext;
using MunicipleComplaintMgmtSys.API.DTOs;
using MunicipleComplaintMgmtSys.API.Enums;
using MunicipleComplaintMgmtSys.API.Models;

namespace MunicipleComplaintMgmtSys.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfficialController : Controller
    {
        private readonly ComplaintDBContext _dbContext;
        public OfficialController(ComplaintDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        [HttpDelete("delete-complaint/{complaintId}")]
        public async Task<IActionResult> DeleteComplaint(Guid complaintId)
        {
            var complaint = await _dbContext.Complaints.FirstOrDefaultAsync(c => c.ComplaintId == complaintId);
            if (complaint == null) return BadRequest(new { message = "Complaint not found" });

            var complaintFedback = await _dbContext.Feedbacks.FirstOrDefaultAsync(f => f.ComplaintId == complaint.ComplaintId);
            if(complaintFedback != null) _dbContext.Feedbacks.Remove(complaintFedback);

            var workUpdates = await _dbContext.WorkUpdates.Where(wu => wu.ComplaintId == complaint.ComplaintId).ToListAsync();
            if(workUpdates != null) _dbContext.WorkUpdates.RemoveRange(workUpdates);

            _dbContext.Complaints.Remove(complaint);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Complaint Deleted succesfully" });
        }


        [HttpPut("assign-complaint/{complaintId}")]
        public async Task<IActionResult> AssignComplaint(Guid complaintId, [FromBody] AssignComplaintDto dto)
        {
            var complaint = await _dbContext.Complaints.FindAsync(complaintId);
            if (complaint == null) return NotFound(new { message = "Complaint not found" });

            var worker = await _dbContext.Workers.FindAsync(dto.WorkerId);
            if (worker == null) return NotFound(new { message = "Worker not found" });

            complaint.AssignedWorkerId = dto.WorkerId;
            complaint.CurrentStatus = ComplaintStatus.Assigned;
            complaint.UpdatedAt = DateTime.Now;

            _dbContext.Complaints.Update(complaint);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Complaint assigned successfully" });
        }

        [HttpGet("departments-overview/{userId}")]
        public async Task<IActionResult> GetDepartmentsOverview(Guid userId)
        {
            var official = await _dbContext.Officials
                            .Include(o=>o.OfficialDepartments)
                            .ThenInclude(od=>od.Department)
                            .FirstOrDefaultAsync(o => o.UserId == userId);

            if (official == null)
                return NotFound(new { message = "Official Not found" });

            var departmentIds = official.OfficialDepartments.Select(od => od.DepartmentId).ToList();
            try
            {
                var departments = await _dbContext.Departments.Where(d=> departmentIds.Contains(d.DepartmentId))
                    .Select(d => new DepartmentOverviewDto
                    {
                        DepartmentId = d.DepartmentId,
                        DepartmentName = d.DepartmentName,
                        TotalComplaints = _dbContext.Complaints.Count(c => c.DepartmentId == d.DepartmentId),
                        PendingComplaints = _dbContext.Complaints
                            .Count(c => c.DepartmentId == d.DepartmentId && c.CurrentStatus != ComplaintStatus.Resolved && c.CurrentStatus != ComplaintStatus.Closed),
                        AssignedComplaints = _dbContext.Complaints
                            .Count(c => c.DepartmentId == d.DepartmentId && c.CurrentStatus == ComplaintStatus.Assigned),
                        ResolvedComplaints = _dbContext.Complaints
                            .Count(c => c.DepartmentId == d.DepartmentId && (c.CurrentStatus == ComplaintStatus.Resolved || c.CurrentStatus == ComplaintStatus.Closed)),
                        OverdueComplaints = _dbContext.Complaints
                            .Count(c => c.DepartmentId == d.DepartmentId && c.SlaDueAt < DateTime.Now &&
                                  c.CurrentStatus != ComplaintStatus.Resolved && c.CurrentStatus != ComplaintStatus.Closed)
                    })
                    .ToListAsync();

                return Ok(departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving departments overview", error = ex.Message });
            }
        }

        [HttpGet("workers-by-department/{departmentId}")]
        public async Task<IActionResult> GetWorkersByDepartment(int departmentId)
        {
            try
            {
                var workers = await _dbContext.Workers
                    .Include(w => w.User)
                    .Where(w => w.DepartmentId == departmentId)
                    .Select(w => new WorkerDto
                    {
                        WorkerId = w.WorkerId,
                        FullName = w.User.FullName,
                        DepartmentId = w.DepartmentId,
                        IsActive = w.User.IsActive,
                        CurrentAssignments = _dbContext.Complaints
                            .Count(c => c.AssignedWorkerId == w.WorkerId &&
                                  c.CurrentStatus != ComplaintStatus.Resolved &&
                                  c.CurrentStatus != ComplaintStatus.Closed)
                    })
                    .OrderBy(w => w.CurrentAssignments)
                    .ToListAsync();

                return Ok(workers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving workers", error = ex.Message });
            }
        }

        [HttpGet("complaint-summary-by-department/{departmentId}")]
        public async Task<IActionResult> GetComplaintSummaryByDepartment(int departmentId)
        {
            try
            {
                var complaints = await _dbContext.Complaints
                    .Include(c => c.Citizen)
                    .Include(c => c.Category)
                    .Include(c => c.SubCategory)
                    .Include(c => c.AssignedWorker)
                        .ThenInclude(w => w.User)
                    .Where(c => c.DepartmentId == departmentId)
                    .Select(c => new ComplaintSummaryDto
                    {
                        ComplaintId = c.ComplaintId,
                        TicketNo = c.TicketNo,
                        CitizenName = c.Citizen.FullName,
                        CategoryName = c.Category.CategoryName,
                        SubCategoryName = c.SubCategory != null ? c.SubCategory.SubCategoryName : "",
                        Description = c.Description,
                        CurrentStatus = c.CurrentStatus.ToString(),
                        CreatedAt = c.CreatedAt,
                        SlaDueAt = c.SlaDueAt,
                        IsOverdue = c.SlaDueAt < DateTime.Now &&
                                  c.CurrentStatus != ComplaintStatus.Resolved &&
                                  c.CurrentStatus != ComplaintStatus.Closed,
                        AssignedWorkerName = c.AssignedWorker != null ? c.AssignedWorker.User.FullName : null
                    })
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving complaint summary", error = ex.Message });
            }
        }

        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = new DashboardStatsDto
                {
                    TotalComplaints = await _dbContext.Complaints.CountAsync(),
                    PendingComplaints = await _dbContext.Complaints
                        .CountAsync(c => c.CurrentStatus == ComplaintStatus.Pending),
                    AssignedComplaints = await _dbContext.Complaints
                        .CountAsync(c => c.CurrentStatus == ComplaintStatus.Assigned),
                    ResolvedComplaints = await _dbContext.Complaints
                        .CountAsync(c => c.CurrentStatus == ComplaintStatus.Resolved),
                    OverdueComplaints = await _dbContext.Complaints
                        .CountAsync(c => c.SlaDueAt < DateTime.Now &&
                                   c.CurrentStatus != ComplaintStatus.Resolved &&
                                   c.CurrentStatus != ComplaintStatus.Closed),
                    TotalDepartments = await _dbContext.Departments.CountAsync(),
                    ActiveWorkers = await _dbContext.Workers.Include(c=>c.User).CountAsync(w => w.User.IsActive)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving dashboard stats", error = ex.Message });
            }
        }

        //[HttpPut("bulk-assign-complaints")]
        //public async Task<IActionResult> BulkAssignComplaints([FromBody] BulkAssignDto dto)
        //{
        //    if (dto?.ComplaintIds == null || !dto.ComplaintIds.Any())
        //        return BadRequest(new { message = "No complaints selected" });

        //    try
        //    {
        //        var worker = await _dbContext.Workers.FindAsync(dto.WorkerId);
        //        if (worker == null) return NotFound(new { message = "Worker not found" });

        //        var complaints = await _dbContext.Complaints
        //            .Where(c => dto.ComplaintIds.Contains(c.ComplaintId))
        //            .ToListAsync();

        //        if (!complaints.Any())
        //            return NotFound(new { message = "No valid complaints found" });

        //        foreach (var complaint in complaints)
        //        {
        //            if (complaint.CurrentStatus == ComplaintStatus.Pending)
        //            {
        //                complaint.AssignedWorkerId = dto.WorkerId;
        //                complaint.CurrentStatus = ComplaintStatus.Assigned;
        //                complaint.UpdatedAt = DateTime.Now;
        //            }
        //        }

        //        _dbContext.Complaints.UpdateRange(complaints);
        //        await _dbContext.SaveChangesAsync();

        //        var assignedCount = complaints.Count(c => c.AssignedWorkerId == dto.WorkerId);

        //        return Ok(new
        //        {
        //            message = $"{assignedCount} complaints assigned successfully",
        //            assignedCount = assignedCount
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Error assigning complaints", error = ex.Message });
        //    }
        //}


        [HttpGet("complaints-export/{departmentId}")]
        public async Task<IActionResult> ExportComplaintsByDepartment(int departmentId)
        {
            try
            {
                var complaints = await _dbContext.Complaints
                    .Include(c => c.Citizen)
                    .Include(c => c.Category)
                    .Include(c => c.SubCategory)
                    .Include(c => c.Department)
                    .Include(c => c.AssignedWorker)
                        .ThenInclude(w => w.User)
                    .Where(c => c.DepartmentId == departmentId)
                    .Select(c => new ComplaintExportDto
                    {
                        TicketNo = c.TicketNo,
                        CitizenName = c.Citizen.FullName,
                        DepartmentName = c.Department.DepartmentName,
                        CategoryName = c.Category.CategoryName,
                        SubCategoryName = c.SubCategory != null ? c.SubCategory.SubCategoryName : "",
                        Description = c.Description,
                        CurrentStatus = c.CurrentStatus.ToString(),
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        SlaDueAt = c.SlaDueAt,
                        AssignedWorkerName = c.AssignedWorker != null ? c.AssignedWorker.User.FullName : "",
                        IsOverdue = c.SlaDueAt < DateTime.Now &&
                                  c.CurrentStatus != ComplaintStatus.Resolved &&
                                  c.CurrentStatus != ComplaintStatus.Closed
                    })
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error exporting complaints", error = ex.Message });
            }
        }

        [HttpPost("reassign-complaint/{complaintId}")]
        public async Task<IActionResult> ReassignComplaint(Guid complaintId, [FromBody] ReassignComplaintDto dto)
        {
            try
            {
                var complaint = await _dbContext.Complaints.FindAsync(complaintId);
                if (complaint == null) return NotFound(new { message = "Complaint not found" });

                var newWorker = await _dbContext.Workers.FindAsync(dto.NewWorkerId);
                if (newWorker == null) return NotFound(new { message = "Worker not found" });

                complaint.AssignedWorkerId = dto.NewWorkerId;
                complaint.UpdatedAt = DateTime.Now;

                // Optionally add a note about reassignment
                if (!string.IsNullOrEmpty(dto.Reason))
                {
                    // Add reassignment history/note logic here if you have such a table
                }

                _dbContext.Complaints.Update(complaint);
                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "Complaint reassigned successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error reassigning complaint", error = ex.Message });
            }
        }

        [HttpGet("GetAllUnassignedOfficialsForDepartment/{departmentId}")]
        public async Task<IActionResult> GetAllOfficials(int departmentId)
        {
            var officials = await _dbContext.Officials
                .Include(o=>o.User)
                .Include(o => o.OfficialDepartments)
                    .ThenInclude(o=>o.Department)
                .Where(o=>!o.OfficialDepartments.Any(od=>od.DepartmentId == departmentId))
                .Select(s => new
                {
                    s.OfficialId,
                    s.User.FullName,
                    departmentInfo = s.OfficialDepartments
                        .Select(o=>new DepartmentInfoDto
                        {
                            DepartmentId = o.DepartmentId,
                            DepartmentName = o.Department != null ? o.Department.DepartmentName : string.Empty
                        }).ToList()
                }).ToListAsync();

            return Ok(officials);
        }

        [HttpGet("assignedOfficialsByDepartmentId/{departmentId}")]
        public async Task<IActionResult> GetAssignedOfficials(int departmentId)
        {
            var assignedOfficials = await _dbContext.Officials
                .Include(o => o.User)
                .Include(o => o.OfficialDepartments)
                    .ThenInclude(od => od.Department)
                .Where(s => s.OfficialDepartments.Any(o => o.DepartmentId == departmentId))
                .Select(o => new
                {
                    o.OfficialId,
                    o.User.Email,
                    o.User.FullName,
                    
                })
                .ToListAsync();

            return Ok(assignedOfficials);
        }
    }
}
