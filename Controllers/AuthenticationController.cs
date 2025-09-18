using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MunicipleComplaintMgmtSys.API.ComplaintContext;
using MunicipleComplaintMgmtSys.API.DTOs;
using MunicipleComplaintMgmtSys.API.Helper;
using MunicipleComplaintMgmtSys.API.Models;

namespace MunicipleComplaintMgmtSys.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ComplaintDBContext _dbContext;
        private readonly JwtService _jwtService;
        public AuthenticationController(ComplaintDBContext dBContext, JwtService jwtService)
        {
            _dbContext = dBContext;
            _jwtService = jwtService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            var exists = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (exists != null) {
                return BadRequest(new {message = "Email already exists"});
            }

            string hash = AuthHelper.HashPassword(dto.Password);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = hash,
                Role = dto.Role,
                Phone = dto.Phone,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            if (dto.Role == Enums.UserRole.Worker && dto.DepartmentId.HasValue)
            {
                var worker = new Worker
                {
                    WorkerId = Guid.NewGuid(),
                    DepartmentId = dto.DepartmentId.Value,
                    IsAvailable = true,
                    User = user
                };

                _dbContext.Workers.Add(worker);
                await _dbContext.SaveChangesAsync();
            }
            else if (dto.Role == Enums.UserRole.Official && dto.DepartmentId.HasValue)
            {
                var official = new Official
                {
                    OfficialId = Guid.NewGuid(),
                    DepartmentId = dto.DepartmentId.Value,
                    User = user
                };

                _dbContext.Officials.Add(official);
                await _dbContext.SaveChangesAsync();
            }

            var userForToken = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (userForToken == null) return BadRequest(new { message = "User is not registered yet!... try to login after some time" });
            var token = _jwtService.GenerateToken(user);
            return Ok(new {
                message = "User registered successfully",
                token = token
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.EmailId);
            if(user == null)
            {
                return Unauthorized(new {message = "Invalid email or password" });
            }

            if (!AuthHelper.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password" });

            var token = _jwtService.GenerateToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(response);
        }


        [HttpPut("update-profile/{userId}")]
        public async Task<IActionResult> UpdateProfile(Guid userId, [FromBody] UpdateProfileDto dto)
        {
            var userExist = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (userExist == null) return NotFound(new { message = " User not found" });

            userExist.FullName = dto.FullName;
            userExist.Email = dto.Email;
            userExist.Phone = dto.Phone;

            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "User updated successfully" });
        }



        [HttpGet("user-profile/{userId}")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(Guid userId)
        {
            var user = await _dbContext.Users
                .Include(u => u.WorkerProfile)
                    .ThenInclude(w => w.Department)
                .Include(u => u.Complaints)
                .Include(u => u.Complaints)
                    .ThenInclude(c => c.Feedback)
                .Include(u => u.Complaints)
                    .ThenInclude(c => c.Department)
                .Include(u => u.Complaints)
                    .ThenInclude(c => c.AssignedWorker)
                .Include(u => u.Complaints)
                    .ThenInclude(c => c.Category)
                .Include(u => u.Complaints)
                    .ThenInclude(c => c.SubCategory)
                .Include(u => u.Complaints)
                    .ThenInclude(c => c.Attachments)
                .Include(u => u.Complaints)
                    .ThenInclude(c => c.Feedback)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            var official = await _dbContext.Set<Official>()
                .Include(o => o.Department)
                .FirstOrDefaultAsync(o => o.UserId == userId);

            var totalComplaints = user.Complaints.Count;
            var resolved = user.Complaints.Count(c => c.CurrentStatus == Enums.ComplaintStatus.Resolved || c.CurrentStatus == Enums.ComplaintStatus.Closed);
            var closed = user.Complaints.Count(c => c.CurrentStatus == Enums.ComplaintStatus.Closed);
            var pending = user.Complaints.Count(c => c.CurrentStatus != Enums.ComplaintStatus.Resolved && c.CurrentStatus != Enums.ComplaintStatus.Closed);

            var allFeedbacks = user.Complaints
                .Where(c => c.Feedback != null)
                .Select(c => c.Feedback!)
                .ToList();

            var dto = new UserProfileDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,

                // Worker Profile
                IsWorkerAvailable = user.WorkerProfile?.IsAvailable,
                WorkerDepartmentId = user.WorkerProfile?.DepartmentId,
                WorkerDepartmentName = user.WorkerProfile?.Department?.DepartmentName,

                // Official Profile
                OfficialDepartmentId = official?.DepartmentId,
                OfficialDepartmentName = official?.Department?.DepartmentName,

                // Complaints
                TotalComplaints = totalComplaints,
                PendingComplaints = pending,
                ResolvedComplaints = resolved,
                ClosedComplaints = closed,

                // Feedbacks
                TotalFeedbacks = allFeedbacks.Count,
                AverageRating = allFeedbacks.Count > 0 ? allFeedbacks.Average(f => f.Rating) : null
            };

            return Ok(dto);
        }
    }
}
