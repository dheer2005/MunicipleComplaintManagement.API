using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MunicipleComplaintMgmtSys.API.ComplaintContext;
using MunicipleComplaintMgmtSys.API.DTOs;
using MunicipleComplaintMgmtSys.API.Models;

namespace MunicipleComplaintMgmtSys.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : Controller
    {
        private readonly ComplaintDBContext _dBContext;

        public DepartmentController(ComplaintDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] DepartmentDto dto)
        {
            var exists = await _dBContext.Departments.FirstOrDefaultAsync(d => d.DepartmentName == dto.DepartmentName);
            if (exists != null) 
            {
                return Ok(new { message = "Department already exist" });
            }

            var department = new Department
            {
                DepartmentName = dto.DepartmentName,
            };

            _dBContext.Departments.Add(department);
            await _dBContext.SaveChangesAsync();
            return Ok(new {message = "Department created successfully"});
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _dBContext.Departments.FirstOrDefaultAsync(d=>d.DepartmentId == id);
            if (department == null)
                return BadRequest(new { message = "Department deleted successfully" });

            _dBContext.Departments.Remove(department);
            await _dBContext.SaveChangesAsync();

            return Ok(new {message = "Department deleted successfully"});
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, string departmentName)
        {
            var department = await _dBContext.Departments.FirstOrDefaultAsync(d => d.DepartmentId == id);
            if (department == null)
            {
                return BadRequest(new { message = "department not found" });
            }

            department.DepartmentName = departmentName;
            await _dBContext.SaveChangesAsync();

            return Ok(new {message = "Department updated"});
        }

        [HttpGet("getAllDepartments")]
        public async Task<IActionResult> GetDepartments()
        {
            var departList = await _dBContext.Departments
                .Select(d=> new
                {
                    d.DepartmentId,
                    d.DepartmentName
                })
                .ToListAsync();

            return Ok(departList);
        }


        [HttpPost("categories/create")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            var exists = await _dBContext.Categories.AnyAsync(d => d.CategoryName == dto.CategoryName);
            if (exists) return BadRequest(new { message = "Category already exists" });

            var category = new Category
            {
                DepartmentId = dto.DepartmentId,
                CategoryName = dto.CategoryName,
                DefaultSlaHours = dto.DefaultSlaHours
            };
            _dBContext.Categories.Add(category);
            await _dBContext.SaveChangesAsync();

            return Ok(new {message = "Category created"});
        }

        [HttpGet("categories/departmentId/{departmentId}")]
        public async Task<IActionResult> CategoriesByDepartmentId(int departmentId)
        {
            var categories = await _dBContext.Categories.Where(c=>c.DepartmentId == departmentId)
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName
                }).ToListAsync();

            return Ok(categories);
        }

        [HttpPost("category/sub-category/create")]
        public async Task<IActionResult> CreateSubCategory([FromBody] CreateSubCategoryDto dto)
        {
            var subCategoryExist = await _dBContext.SubCategories.AnyAsync(s => s.CategoryId == dto.CategoryId && s.SubCategoryName == dto.SubCategoryName);
            if (subCategoryExist) return BadRequest(new { message = "Sub-category already exist" });

            var subCategory = new SubCategory
            {
                CategoryId = dto.CategoryId,
                SubCategoryName = dto.SubCategoryName,
                SlaHours = dto.SlaHours
            };

            _dBContext.SubCategories.Add(subCategory);
            await _dBContext.SaveChangesAsync();

            return Ok(new { message = "New Sub-category created" });
        }

        [HttpGet("Sub-category/by-CategoryId/{categoryId}")]
        public async Task<IActionResult> SubCategroyByCategoryId(int categoryId)
        {
            var subCategories = await _dBContext.SubCategories.Where(s=>s.CategoryId == categoryId)
                .Select(s => new
                {
                    s.SubCategoryId,
                    s.SubCategoryName
                })
                .ToListAsync();

                return Ok(subCategories);
        }
    }
}
