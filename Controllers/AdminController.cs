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
    public class AdminController : Controller
    {
        private readonly ComplaintDBContext _dbContext;

        public AdminController(ComplaintDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var complaints = await _dbContext.Complaints.ToListAsync();
            var users = await _dbContext.Users.ToListAsync();
            var departments = await _dbContext.Departments.ToListAsync();

            var totalComplaints = complaints.Count();
            
            var complaintsByStatus = complaints
                .GroupBy(c => c.CurrentStatus)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                });

            var totalDepartments = departments.Count();
            var totalCitizens = users.Count(c => c.Role == UserRole.Citizen);
            var totalWorkers = users.Count(c => c.Role == UserRole.Worker);
            var totalActiveWorkers = users.Count(c => c.IsActive == true && c.Role == UserRole.Worker);
            var totalOfficials = users.Count(c => c.Role == UserRole.Official);

            int overDueComplaints = complaints.Count(c => (c.CurrentStatus != ComplaintStatus.Resolved && c.CurrentStatus != ComplaintStatus.Closed) && c.SlaDueAt < DateTime.Now);

            var result = new
            {
                TotalComplaints = totalComplaints,
                ComplaintByStatus = complaintsByStatus,
                OverDueComplaints = overDueComplaints,
                TotalDepartments = totalDepartments,
                TotalWorkers = totalWorkers,
                ActiveWorkers = totalActiveWorkers,
                TotalCitizens = totalCitizens,
                TotalOfficials = totalOfficials
            };

            return Ok(result);
        }

        [HttpGet("departments-overview/{departmentId}")]
        public async Task<IActionResult> GetDepartmentsOverview(int departmentId)
        {
            var complaints = await _dbContext.Complaints
                                .Include(c => c.Department)
                                .Where(c => c.Department!.DepartmentId == departmentId)
                                .ToListAsync();

            if (!complaints.Any())
            {
                var emptyResult = new
                {
                    DepartmentId = departmentId,
                    TotalComplaint = 0,
                    ComplaintStatus = new List<object>(),
                    OverDueComplaint = 0,
                    SLACompliance = new
                    {
                        Met = 0,
                        Missed = 0
                    }
                };
                return Ok(emptyResult);
            }

            var totalComplaints = complaints.Count;

            var complaintsByStatus = complaints
                .GroupBy(c => c.CurrentStatus)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                }).ToList();

            int metSla = complaints.Count(c => (c.CurrentStatus == ComplaintStatus.Resolved || c.CurrentStatus == ComplaintStatus.Closed) && c.UpdatedAt <= c.SlaDueAt);
            int missedSla = complaints.Count(c => (c.CurrentStatus == ComplaintStatus.Resolved || c.CurrentStatus == ComplaintStatus.Closed) && c.UpdatedAt > c.SlaDueAt);

            int overDueComplaints = complaints.Count(c => (c.CurrentStatus != ComplaintStatus.Resolved && c.CurrentStatus != ComplaintStatus.Closed) && c.SlaDueAt < DateTime.Now);

            var result = new
            {
                DepartmentId = departmentId,
                TotalComplaint = totalComplaints,
                ComplaintStatus = complaintsByStatus,
                OverDueComplaint = overDueComplaints,
                SLACompliance = new
                {
                    Met = metSla,
                    Missed = missedSla
                }
            };

            return Ok(result);                    
        }

        [HttpGet("Citizens")]
        public async Task<IActionResult> CitizensInfo()
        {
            var citizens = await _dbContext.Users
                .Where(u => u.Role == UserRole.Citizen)
                .Select(c => new
                {
                    c.UserId,
                    c.FullName,
                    c.Email,
                    c.Phone,
                    TotalComplaints = _dbContext.Complaints.Count(comp => comp.CitizenId == c.UserId),
                    TotalResolvedComplaints = _dbContext.Complaints.Count(comp => comp.CitizenId == c.UserId && (comp.CurrentStatus == ComplaintStatus.Resolved || comp.CurrentStatus == ComplaintStatus.Closed))
                })
                .ToListAsync();

            return Ok(citizens);
        }

        [HttpGet("workers")]
        public async Task<IActionResult> GetWorkerInfo()
        {
            var workers = await _dbContext.Users
                .Include(u=>u.WorkerProfile)
                .Where(w => w.Role == UserRole.Worker)
                .Select(c => new
                {
                    c.UserId,
                    c.FullName,
                    c.Email,
                    c.Phone,
                    TotalAssignedComplaints = _dbContext.Complaints.Count(cc=>cc.AssignedWorkerId == c.WorkerProfile.WorkerId),
                    TotalResolvedComplaints = _dbContext.Complaints.Count(cc=> cc.AssignedWorkerId == c.WorkerProfile.WorkerId && (cc.CurrentStatus == ComplaintStatus.Resolved || cc.CurrentStatus == ComplaintStatus.Closed)),
                    TotalPendingComplaints = _dbContext.Complaints.Count(cc=> cc.AssignedWorkerId == c.WorkerProfile.WorkerId && (cc.CurrentStatus != ComplaintStatus.Resolved && cc.CurrentStatus != ComplaintStatus.Closed))
                })
                .ToListAsync();

            return Ok(workers);
        }

        [HttpGet("Official")]
        public async Task<IActionResult> GetOfficialInfo()
        {
            var official = await _dbContext.Users
                .Where(o => o.Role == UserRole.Official)
                .ToListAsync();
            return Ok(official);
        }

        [HttpDelete("deleteUsers/{userId}")]
        public async Task<IActionResult> DeleteUsers(Guid userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            if(user.Role == UserRole.Citizen)
            {
                _dbContext.Users.Remove(user);
            }else if(user.Role == UserRole.Worker)
            {
                var worker = await _dbContext.Workers.FirstOrDefaultAsync(w => w.UserId == userId);
                if (worker == null) return NotFound(new { message = "Worker not found" });

                _dbContext.Workers.Remove(worker);
                _dbContext.Users.Remove(user);
            }else if(user.Role == UserRole.Official)
            {
                var official = await _dbContext.Officials.FirstOrDefaultAsync(o => o.UserId == userId);
                if (official == null) return NotFound(new { message = "Official not found" });

                _dbContext.Officials.Remove(official);
                _dbContext.Users.Remove(user);
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }

        [HttpGet("recent-complaints")]
        public async Task<IActionResult> GetRecentComplaints()
        {
            var recentSubmittedComplaints = await _dbContext.Complaints
                            .OrderByDescending(c => c.CreatedAt)
                            .Take(5)
                            .ToListAsync();

            var recentResolvedComplaints = await _dbContext.Complaints
                            .Where(c => c.CurrentStatus == ComplaintStatus.Resolved || c.CurrentStatus == ComplaintStatus.Closed)
                            .OrderByDescending(c => c.UpdatedAt)
                            .Take(5)
                            .ToListAsync();

            var result = new
            {
                RecentSubmittedComplaints = recentSubmittedComplaints,
                RecentResolvedComplaints = recentResolvedComplaints
            };

            return Ok(result);
        }

        [HttpGet("Get-complaints")]
        public async Task<IActionResult> GetComplaintByUserId()
        {
            var complaints = await _dbContext.Complaints
                .Include(c => c.Attachments)
                .Include(c => c.Category)
                .Include(c => c.SubCategory)
                .Include(c => c.Department)
                .Include(c => c.Citizen)
                .Include(c => c.AssignedWorker)
                    .ThenInclude(w => w.User)
                .Include(c => c.Feedback)
                .ToListAsync();

            if (complaints == null || !complaints.Any())
                return NotFound(new { message = "No complaints found for this department" });

            var dtos = complaints.Select(complaint => new ComplaintDto
            {
                ComplaintId = complaint.ComplaintId,
                TicketNo = complaint.TicketNo,
                Description = complaint.Description,
                CurrentStatus = complaint.CurrentStatus.ToString(),
                IsReopened = complaint.IsReopened,
                CreatedAt = complaint.CreatedAt,
                UpdatedAt = complaint.UpdatedAt,
                SlaDueAt = complaint.SlaDueAt,
                Latitude = complaint.Latitude,
                Longitude = complaint.Longitude,
                AddressText = complaint.AddressText,

                CitizenName = complaint.Citizen?.FullName,
                CategoryName = complaint.Category?.CategoryName,
                SubCategoryName = complaint.SubCategory?.SubCategoryName,
                DepartmentName = complaint.Department?.DepartmentName,

                AssignedWorkerName = complaint.AssignedWorker?.User?.FullName,

                Attachments = complaint.Attachments.Select(a => new ComplaintAttachmentDto
                {
                    AttachmentId = a.AttachmentId,
                    ImageUrl = a.ImageUrl,
                    AttachmentType = a.AttachmentType.ToString(),
                    UploadedAt = a.UploadedAt
                }).ToList(),

                Feedback = complaint.Feedback == null ? null : new FeedbackDto
                {
                    IsSatisfied = complaint.Feedback.IsSatisfied,
                    Rating = complaint.Feedback.Rating,
                    Comments = complaint.Feedback.Comment
                }
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("assignOfficialToDepartment/{officialId}/{departmentId}")]
        public async Task<IActionResult> AssignOfficialToDepartment(Guid officialId, int departmentId)
        {
            var official = await _dbContext.Officials.FirstOrDefaultAsync(o => o.OfficialId == officialId);

            if (official == null)
                return NotFound(new { message = "Official not found" });

            var alreadyAssigned = await _dbContext.OfficialDepartments.FirstOrDefaultAsync(od => od.OfficialId == officialId && od.DepartmentId == departmentId);
            if (alreadyAssigned != null)
                return BadRequest(new { message = "Official already assigned to that department" });

            var AssignNewOfficial = new OfficialDepartment
            {
                OfficialId = officialId,
                DepartmentId = departmentId
            };

            _dbContext.OfficialDepartments.Add(AssignNewOfficial);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "New Official assigned successfully" });
        }

        [HttpGet("unAssignOfficialFromDepartment/{officialId}/{departmentId}")]
        public async Task<IActionResult> UnAssignOfficialFromDepartment(Guid officialId, int departmentId)
        {
            var OfficialExist = await _dbContext.OfficialDepartments.FirstOrDefaultAsync(od => od.OfficialId == officialId);

            if (OfficialExist == null)
                return NotFound(new { message = "Official unassigned already" });

            _dbContext.OfficialDepartments.Remove(OfficialExist);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Official unAssigned successfully" });
        }

        [HttpDelete("categories/delete/{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId);
            if (category == null) return NotFound(new { mesdsage = "Category not found" });

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Category deleted successfully" });
        }

        [HttpDelete("category/sub-categories/delete/{subCategoryId}")]
        public async Task<IActionResult> DeleteSubCategory(int subCategoryId)
        {
            var subCategory = await _dbContext.SubCategories.FirstOrDefaultAsync(sc => sc.SubCategoryId == subCategoryId);
            if (subCategory == null) return NotFound(new { messsage = "Sub category not found" });

            _dbContext.SubCategories.Remove(subCategory);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Sub category deleted successfully" });
        }

        
    }
}
