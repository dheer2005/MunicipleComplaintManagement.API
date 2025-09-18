using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MunicipleComplaintMgmtSys.API.ComplaintContext;
using MunicipleComplaintMgmtSys.API.DTOs;
using MunicipleComplaintMgmtSys.API.Enums;
using MunicipleComplaintMgmtSys.API.InterfaceImplementation;
using MunicipleComplaintMgmtSys.API.Interfaces;
using MunicipleComplaintMgmtSys.API.Models;

namespace MunicipleComplaintMgmtSys.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplaintController : Controller
    {
        private readonly ComplaintDBContext _dbContext;
        private readonly IImageStorage _cloudinary;
        public ComplaintController(ComplaintDBContext dBContext, IImageStorage cloudinary)
        {
            _dbContext = dBContext;
            _cloudinary = cloudinary;
        }


        [HttpPost("Create-complaint")]
        public async Task<IActionResult> CreateComplaint([FromForm] ComplaintCreateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Invalid Data" });
            var category = await _dbContext.Categories.FirstOrDefaultAsync(d => d.CategoryId == dto.CategoryId);
            var subCategory = await _dbContext.SubCategories.FirstOrDefaultAsync(s => s.SubCategoryId == dto.SubCategoryId);

            var complaint = new Complaint
            {
                ComplaintId = Guid.NewGuid(),
                TicketNo = $"CMP-{DateTime.Now.Ticks}",
                CitizenId = dto.CitizenId,
                DepartmentId = dto.DepartmentId,
                CategoryId = dto.CategoryId,
                SubCategoryId = dto.SubCategoryId,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                AddressText = dto.AddressText,
                Description = dto.Description,
                CurrentStatus = dto.CurrentStatus,
                CreatedAt = DateTime.Now,
                SlaDueAt = subCategory?.SlaHours != null ? DateTime.Now.AddHours(subCategory.SlaHours) : DateTime.Now.AddHours(category!.DefaultSlaHours)
            };

            _dbContext.Complaints.Add(complaint);

            if (dto.Attachments != null && dto.Attachments.Any()) 
            {
                foreach (var file in dto.Attachments) 
                {
                    var imageUrl = await _cloudinary.UploadImageAsync(file);

                    var attachment = new ComplaintAttachment
                    {
                        ComplaintId = complaint.ComplaintId,
                        UploadedByUserId = dto.CitizenId,
                        ImageUrl = imageUrl != null ? imageUrl : "",
                        AttachmentType = dto.AttachmentType,
                        UploadedAt = DateTime.Now,
                    };

                    _dbContext.ComplaintAttachments.Add(attachment);
                }
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Complaint created successfully",
                TicketNo = complaint.TicketNo,
                ComplaintId = complaint.ComplaintId
            });
        }

        [HttpPut("edit-complaint/{complaintId}")]
        public async Task<IActionResult> EditComplaint(Guid complaintId, [FromForm] ComplaintCreateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Invalid Data" });

            var existComplaint = await _dbContext.Complaints.FirstOrDefaultAsync(c => c.ComplaintId == complaintId);

            if (existComplaint == null) return NotFound(new { message = "Complaint not found" });

            var category = await _dbContext.Categories.FirstOrDefaultAsync(d => d.CategoryId == dto.CategoryId);
            var subCategory = await _dbContext.SubCategories.FirstOrDefaultAsync(s => s.SubCategoryId == dto.SubCategoryId);

            existComplaint.CitizenId = dto.CitizenId;
            existComplaint.DepartmentId = dto.DepartmentId;
            existComplaint.CategoryId = dto.CategoryId;
            existComplaint.SubCategoryId = dto.SubCategoryId;
            existComplaint.Latitude = dto.Latitude;
            existComplaint.Longitude = dto.Longitude;
            existComplaint.AddressText = dto.AddressText;
            existComplaint.Description = dto.Description;
            existComplaint.CurrentStatus = dto.CurrentStatus;
            existComplaint.CreatedAt = DateTime.Now;
            existComplaint.SlaDueAt = subCategory?.SlaHours != null ? DateTime.Now.AddHours(subCategory.SlaHours) : DateTime.Now.AddHours(category!.DefaultSlaHours);

            _dbContext.Complaints.Update(existComplaint);

            if (dto.Attachments != null && dto.Attachments.Any())
            {
                foreach (var file in dto.Attachments)
                {
                    var imageUrl = await _cloudinary.UploadImageAsync(file);

                    var attachment = new ComplaintAttachment
                    {
                        ComplaintId = complaintId,
                        UploadedByUserId = dto.CitizenId,
                        ImageUrl = imageUrl != null ? imageUrl : "",
                        AttachmentType = dto.AttachmentType,
                        UploadedAt = DateTime.Now,
                    };

                    _dbContext.ComplaintAttachments.Add(attachment);
                }
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Complaint edited successfully",
                TicketNo = existComplaint.TicketNo,
                ComplaintId = complaintId
            });
        }

        [HttpDelete("delete-complaint-attachments/{complaintId}/{attachmentId}")]
        public async Task<IActionResult> deleteComplaintAttachments(Guid complaintId, int attachmentId)
        {
            var attachments = await _dbContext.ComplaintAttachments.Where(ca => ca.ComplaintId == complaintId).ToListAsync();

            if (attachments == null) return NotFound(new { message = "complaint attchments not found" });

            var exist = attachments.Where(a => a.AttachmentId == attachmentId);
            if (exist == null) return NotFound(new { message = " Attachment not found" });

            _dbContext.ComplaintAttachments.RemoveRange(exist);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Complaint attachments delete successfully" });
        }

        [HttpDelete("delete-complaint/{complaintId}")]
        public async Task<IActionResult> DeleteComplaintById(Guid complaintId)
        {
            var complaint = await _dbContext.Complaints.FirstOrDefaultAsync(c => c.ComplaintId == complaintId && c.CurrentStatus == ComplaintStatus.Pending);
            if (complaint == null) return BadRequest(new { message = "Complaint not found" });

            var complaintAttachments = _dbContext.ComplaintAttachments.Where(c => c.ComplaintId == complaint.ComplaintId);

            _dbContext.ComplaintAttachments.RemoveRange(complaintAttachments);
            _dbContext.Complaints.Remove(complaint);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Complaint deleted successfully" });
        }

        [HttpGet("Get-complaint/userId/{userId}")]
        public async Task<IActionResult> GetComplaintByUserId(Guid userId)
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
                .Where(c => c.CitizenId == userId)
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

        [HttpGet("Get-complaint/{complaintId}")]
        public async Task<IActionResult> GetComplaintById(Guid complaintId)
        {
            var complaint = await _dbContext.Complaints
                .Include(c => c.Attachments)
                .Include(c => c.Category)
                .Include(c => c.SubCategory)
                .Include(c => c.Department)
                .Include(c => c.Citizen)
                .Include(c => c.AssignedWorker)
                    .ThenInclude(w=>w.User)
                .Include(c => c.Feedback)
                .FirstOrDefaultAsync(c => c.ComplaintId == complaintId);

            if (complaint == null) return NotFound();

            var dto = new ComplaintDto
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

                Feedback = complaint.Feedback == null ? null : new FeedbackDto
                {
                    IsSatisfied = complaint.Feedback.IsSatisfied,
                    Rating = complaint.Feedback.Rating,
                    Comments = complaint.Feedback.Comment
                }
            };

            return Ok(dto);
        }


        [HttpGet("Get-complaints-by-department/{departmentId}")]
        public async Task<IActionResult> GetComplaintsByDepartment(int departmentId)
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
                .Where(c => c.DepartmentId == departmentId)
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

        [HttpPost("submit-feedback")]
        public async Task<IActionResult> SubmitFeedback([FromBody] FeedbackCreateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Invalid feedback data" });

            var complaint = await _dbContext.Complaints.FindAsync(dto.ComplaintId);
            if (complaint == null) return NotFound(new { message = "Complaint not found" });

            var existingFeedback = await _dbContext.Feedbacks
                .FirstOrDefaultAsync(f => f.ComplaintId == dto.ComplaintId);

            if (existingFeedback != null)
                return BadRequest(new { message = "Feedback already submitted for this complaint" });

            var feedback = new Feedback
            {
                ComplaintId = dto.ComplaintId,
                CitizenId = complaint.CitizenId,
                IsSatisfied = dto.IsSatisfied,
                Rating = dto.Rating,
                Comment = dto.Comments,
                CreatedAt = DateTime.Now
            };

            _dbContext.Feedbacks.Add(feedback);

            if (!dto.IsSatisfied)
            {
                complaint.CurrentStatus = ComplaintStatus.Reopened;
                complaint.IsReopened = true;
                complaint.UpdatedAt = DateTime.Now;
                _dbContext.Complaints.Update(complaint);
            }
            else
            {
                // Mark as closed if satisfied
                complaint.CurrentStatus = ComplaintStatus.Closed;
                complaint.UpdatedAt = DateTime.Now;
                _dbContext.Complaints.Update(complaint);
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Feedback submitted successfully",
                isReopened = !dto.IsSatisfied || dto.Rating < 3
            });
        }

        [HttpPut("reopen-complaint/{complaintId}")]
        public async Task<IActionResult> ReopenComplaint(Guid complaintId)
        {
            var complaint = await _dbContext.Complaints.FindAsync(complaintId);
            if (complaint == null) return NotFound(new { message = "Complaint not found" });

            if (complaint.CurrentStatus != ComplaintStatus.Resolved )
            {
                return BadRequest(new { message = "Only resolved complaints can be reopened" });
            }

            complaint.CurrentStatus = ComplaintStatus.Reopened;
            complaint.IsReopened = true;
            complaint.UpdatedAt = DateTime.Now;

            _dbContext.Complaints.Update(complaint);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Complaint reopened successfully"
            });
        }

        //[HttpPut("update-status/{complaintId}")]
        //public async Task<IActionResult> UpdateStatus(Guid complaintId, [FromBody] StatusUpdateDto dto)
        //{
        //    var complaint = await _dbContext.Complaints.FindAsync(complaintId);
        //    if (complaint == null) return NotFound(new { message = "Complaint not found" });

        //    if(Enum.TryParse<ComplaintStatus>(dto.Status, out var status))
        //    {
        //        complaint.CurrentStatus = status;
        //        complaint.UpdatedAt = DateTime.Now;

        //        // Set SLA due date for certain statuses
        //        //if (status == ComplaintStatus.Assigned || status == ComplaintStatus.InProgress)
        //        //{
        //        //    complaint.SlaDueAt = DateTime.Now.AddDays(7);
        //        //}

        //        _dbContext.Complaints.Update(complaint);
        //        await _dbContext.SaveChangesAsync();

        //        return Ok(new { message = "Status updated successfully" });
        //    }

        //    return BadRequest(new { message = "Invalid Status" });
        //}

    }
}
