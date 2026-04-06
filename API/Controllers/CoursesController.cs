using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")] // http://localhost:5001/api/courses
    [ApiController]
    public class CoursesController(WebContext context) : ControllerBase
    {
        [HttpGet]
        public async Task <ActionResult<List<Course>>> GetCourses()
        {
            return await context.Courses.ToListAsync();
        }
        [HttpGet("{id}")] // api/courses/1
        public async Task <ActionResult<Course>> GetCourse(int id)
        {
            var course = await context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            return course;
        }
    }
}
