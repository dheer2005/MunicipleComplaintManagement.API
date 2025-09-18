using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MunicipleComplaintMgmtSys.API.ComplaintContext;
using MunicipleComplaintMgmtSys.API.DTOs;
using MunicipleComplaintMgmtSys.API.Enums;
using MunicipleComplaintMgmtSys.API.Interfaces;
using MunicipleComplaintMgmtSys.API.Models;

namespace MunicipleComplaintMgmtSys.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkerController : Controller
    {
        private readonly ComplaintDBContext _dbContext;
        private readonly IImageStorage _cloudinary;
        public WorkerController(ComplaintDBContext dBContext, IImageStorage cloudinary)
        {
            _dbContext = dBContext;
            _cloudinary = cloudinary;
        }

        [HttpGet("GetByDepartmentId/{departmentId}")]
        public async Task<IActionResult> WorkersByDepartmentId(int departmentId)
        {
            var workers = await _dbContext.Workers.Where(w => w.DepartmentId == departmentId).ToListAsync();
            return Ok(workers);
        }

        [HttpGet("Get-complaint-for-worker/userId/{userId}")]
        public async Task<IActionResult> GetComplaintByWorkerId(Guid userId)
        {
            var worker = await _dbContext.Workers.FirstOrDefaultAsync(w => w.UserId == userId);

            if (worker == null) return NotFound(new { message = "Worker not found" });


            var complaints = await _dbContext.Complaints
                .Include(c => c.Attachments)
                .Include(c => c.Category)
                .Include(c => c.SubCategory)
                .Include(c => c.Department)
                .Include(c => c.Citizen)
                .Include(c => c.AssignedWorker)
                    .ThenInclude(w => w.User)
                .Include(c => c.Feedback)
                .Where(c => c.AssignedWorkerId == worker.WorkerId && c.DepartmentId == worker.DepartmentId)
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


        [HttpGet("worker-stats/{userId}")]
        public async Task<IActionResult> GetWorkerStats(Guid userId)
        {
            try
            {
                var worker = await _dbContext.Workers.FirstOrDefaultAsync(u => u.UserId == userId);
                var stats = new
                {
                    TotalAssigned = await _dbContext.Complaints.CountAsync(c => c.AssignedWorkerId == worker.WorkerId),
                    Pending = await _dbContext.Complaints.CountAsync(c => c.AssignedWorkerId == worker.WorkerId && (c.CurrentStatus == ComplaintStatus.Pending && c.CurrentStatus == ComplaintStatus.Assigned)),
                    InProgress = await _dbContext.Complaints.CountAsync(c => c.AssignedWorkerId == worker.WorkerId && c.CurrentStatus == ComplaintStatus.InProgress),
                    Resolved = await _dbContext.Complaints.CountAsync(c => c.AssignedWorkerId == worker.WorkerId && (c.CurrentStatus == ComplaintStatus.Resolved || c.CurrentStatus == ComplaintStatus.Closed)),
                    Overdue = await _dbContext.Complaints.CountAsync(c => c.AssignedWorkerId == worker.WorkerId &&
                                                c.SlaDueAt < DateTime.Now && c.CurrentStatus != ComplaintStatus.Resolved && c.CurrentStatus != ComplaintStatus.Closed)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving worker stats", error = ex.Message });
            }
        }

        [HttpGet("worker-recent-tasks/{userId}")]
        public async Task<IActionResult> GetWorkerRecentTasks(Guid userId)
        {
            try
            {
                var worker = await _dbContext.Workers.FirstOrDefaultAsync(w => w.UserId == userId);
                var recentTasks = await _dbContext.Complaints
                    .Include(c => c.Category)
                    .Include(c => c.SubCategory)
                    .Where(c => c.AssignedWorkerId == worker.WorkerId)
                    .OrderByDescending(c => c.UpdatedAt)
                    .Take(5)
                    .Select(n => new
                    {
                        ComplaintId = n.ComplaintId,
                        TicketNo = n.TicketNo,
                        Description = n.Description,
                        Status = n.CurrentStatus.ToString(),
                        AssignedDate = n.UpdatedAt,
                        DueDate = n.SlaDueAt,
                        Priority = (n.SlaDueAt < DateTime.Now) ? "High" :
                                    (EF.Functions.DateDiffDay(n.CreatedAt, DateTime.Now) > 7) ? "High" :
                                    (EF.Functions.DateDiffDay(n.CreatedAt, DateTime.Now) > 3) ? "Medium" :
                                    "Low"
                    })
                    .ToListAsync();

                return Ok(recentTasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving worker stats", error = ex.Message });
            }
        }

        [HttpGet("worker-upcoming-deadlines/{userId}")]
        public async Task<IActionResult> GetWorkerUpcomingDeadlines(Guid userId)
        {
            try
            {
                var worker = await _dbContext.Workers.FirstOrDefaultAsync(w => w.UserId == userId);
                if (worker == null)
                {
                    return NotFound(new { message = "Worker not found" });
                }

                var now = DateTime.Now; // take value outside LINQ

                var complaints = await _dbContext.Complaints
                    .Where(c => c.AssignedWorkerId == worker.WorkerId &&
                                c.SlaDueAt.HasValue &&
                                c.SlaDueAt.Value > now && // upcoming deadlines = future
                                c.CurrentStatus != ComplaintStatus.Resolved &&
                                c.CurrentStatus != ComplaintStatus.Closed)
                    .OrderBy(c => c.SlaDueAt)
                    .Take(5)
                    .ToListAsync();

                // Priority calculation in memory (safe)
                var upcoming = complaints.Select(c => new
                {
                    ComplaintId = c.ComplaintId,
                    TicketNo = c.TicketNo,
                    Description = c.Description,
                    Status = c.CurrentStatus.ToString(),
                    AssignedDate = c.UpdatedAt,
                    DueDate = c.SlaDueAt,
                    Priority =
                        (c.SlaDueAt.HasValue && c.SlaDueAt.Value < now) ? "High" :
                        ((now - c.CreatedAt).TotalDays > 7) ? "High" :
                        ((now - c.CreatedAt).TotalDays > 3) ? "Medium" :
                        "Low"
                });

                return Ok(upcoming);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving worker stats", error = ex.Message });
            }
        }


        [HttpGet("priority-stats/{userId}")]
        public async Task<IActionResult> GetWorkerPriorityStats(Guid userId)
        {
            var worker = await _dbContext.Workers.FirstOrDefaultAsync(w => w.UserId == userId);
            if (worker == null)
            {
                return NotFound(new { message = "Worker not found" });
            }

            var now = DateTime.Now;

            var complaints = await _dbContext.Complaints
                .Where(c => c.AssignedWorkerId == worker.WorkerId && !(c.CurrentStatus == ComplaintStatus.Resolved || c.CurrentStatus == ComplaintStatus.Closed))
                .ToListAsync();

            var high = complaints.Count(c =>
                (c.SlaDueAt.HasValue && c.SlaDueAt.Value < now) ||
                (now - c.CreatedAt).TotalDays > 7
            );

            var medium = complaints.Count(c =>
                (c.SlaDueAt.HasValue && c.SlaDueAt.Value >= now) &&
                (now - c.CreatedAt).TotalDays > 3 &&
                (now - c.CreatedAt).TotalDays <= 7
            );

            var low = complaints.Count(c =>
                (c.SlaDueAt.HasValue && c.SlaDueAt.Value >= now) &&
                (now - c.CreatedAt).TotalDays <= 3
            );

            return Ok(new { high, medium, low });
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
                    ActiveWorkers = await _dbContext.Workers.Include(c => c.User).CountAsync(w => w.User.IsActive)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving dashboard stats", error = ex.Message });
            }
        }

        [HttpPost("add-work-update/{complaintId}")]
        public async Task<IActionResult> AddWorkUpdate(Guid complaintId, [FromForm] StatusUpdateDto dto)
        {
            var complaint = await _dbContext.Complaints.FindAsync(complaintId);
            if (complaint == null) return NotFound();

            var workUpdate = new WorkUpdate
            {
                WorkUpdateId = Guid.NewGuid(),
                ComplaintId = complaintId,
                UpdatedByUserId = dto.UpdatedByUserId,
                Status = dto.Status,
                Notes = dto.Notes,
                CompletionPercentage = dto.CompletionPercentage,
                UpdatedAt = DateTime.Now,
                EstimatedCompletionDate = dto.EstimatedCompletionDate,
                RequiresAdditionalResources = dto.RequiresAdditionalResources,
                AdditionalResourcesNeeded = dto.AdditionalResourcesNeeded ?? ""
            };
            _dbContext.WorkUpdates.Add(workUpdate);

            if(dto.Attachments != null && dto.Attachments.Any())
            {
                foreach(var file in dto.Attachments)
                {
                    var imageUrl = await _cloudinary.UploadImageAsync(file);
                    var attachment = new ComplaintAttachment
                    {
                        ComplaintId = workUpdate.ComplaintId,
                        UploadedByUserId = dto.UpdatedByUserId,
                        ImageUrl = imageUrl != null ? imageUrl : "",
                        AttachmentType = AttachmentType.WorkerProof,
                        UploadedAt = DateTime.Now,
                    };

                    _dbContext.ComplaintAttachments.Add(attachment);
                }
            }


            // Optionally, update Complaint's current status and updatedAt
            complaint.CurrentStatus = Enum.Parse<ComplaintStatus>(dto.Status);
            complaint.UpdatedAt = DateTime.Now;
            complaint.SlaDueAt = workUpdate.EstimatedCompletionDate;
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Work update added" });
        }

        [HttpGet("get-work-history/{complaintId}")]
        public async Task<IActionResult> GetWorkHistory(Guid complaintId)
        {
            var updates = await _dbContext.WorkUpdates
                .Where(wu => wu.ComplaintId == complaintId)
                .OrderBy(wu => wu.UpdatedAt)
                .Include(wu => wu.Attachments)
                .Select(wu => new WorkUpdateDto
                {
                    Status = wu.Status,
                    Notes = wu.Notes,
                    CompletionPercentage = wu.CompletionPercentage,
                    UpdatedAt = wu.UpdatedAt,
                    EstimatedCompletionDate = wu.EstimatedCompletionDate,
                    RequiresAdditionalResources = wu.RequiresAdditionalResources,
                    AdditionalResourcesNeeded = wu.AdditionalResourcesNeeded,
                    Attachments = wu.Attachments
                                    .Select(a=>new ComplaintAttachmentDto
                                    {
                                        AttachmentId = a.AttachmentId,
                                        ImageUrl = a.ImageUrl,
                                        AttachmentType = a.AttachmentType.ToString(),
                                        UploadedAt = a.UploadedAt
                                    })
                                    .ToList()
                })
                .ToListAsync();

            return Ok(updates);

        }

        [HttpGet("Get-complaint/{complaintId}")]
        public async Task<IActionResult> GetComplaintByIdForWorker(Guid complaintId)
        {
            var complaint = await _dbContext.Complaints
                .Include(c => c.Attachments)
                .Include(c => c.Category)
                .Include(c => c.SubCategory)
                .Include(c => c.Department)
                .Include(c => c.Citizen)
                .Include(c => c.AssignedWorker)
                    .ThenInclude(w => w.User)
                .Include(c => c.Feedback)
                .Include(c=>c.WorkUpdates)
                .FirstOrDefaultAsync(c => c.ComplaintId == complaintId);

            if (complaint == null) return NotFound();

            var dto = new WorkerComplaintDto
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

                CitizenName = complaint.Citizen.FullName,
                CategoryName = complaint.Category.CategoryName,
                SubCategoryName = complaint.SubCategory?.SubCategoryName,
                DepartmentName = complaint.Department?.DepartmentName,

                AssignedWorkerName = complaint.AssignedWorker?.User.FullName,

                Attachments = complaint.Attachments.Select(a => new ComplaintAttachmentDto
                {
                    AttachmentId = a.AttachmentId,
                    ImageUrl = a.ImageUrl,
                    AttachmentType = a.AttachmentType.ToString(),
                    UploadedAt = a.UploadedAt
                }).ToList(),

                WorkUpdates = complaint.WorkUpdates
                .Select(wu => new WorkerUpdateDetailDto
                {
                    Notes = wu.Notes,
                    CompletionPercentage = wu.CompletionPercentage,
                    UpdatedAt = wu.UpdatedAt,
                    EstimatedCompletionDate = wu.EstimatedCompletionDate,
                    RequiresAdditionalResources = wu.RequiresAdditionalResources,
                    AdditionalResourcesNeeded = wu.AdditionalResourcesNeeded
                })
                .OrderByDescending(wu => wu.CompletionPercentage)
                .Take(1)
                .ToList(),

                Feedback = complaint.Feedback == null ? null : new FeedbackDto
                {
                    IsSatisfied = complaint.Feedback.IsSatisfied,
                    Rating = complaint.Feedback.Rating,
                    Comments = complaint.Feedback.Comment
                },

            };

            return Ok(dto);
        }


    }
}
