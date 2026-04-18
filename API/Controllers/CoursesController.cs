using System.Security.Claims;
using API.Data;
using API.Entities;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize(Roles = "Admin,Instructor")]
    [Route("api/[controller]")]
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

        [HttpPost]
        [Authorize(Roles = "Admin, Instructor")]
        public async Task<ActionResult<Course>> CreateCourse(CourseDto courseDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var course = new Course
            {
                Title = courseDto.Title,
                Description = courseDto.Description,
                Price = courseDto.Price,
                ThumbnailUrl = courseDto.ThumbnailUrl,
                Language = courseDto.Language,
                CategoryId = courseDto.CategoryId,
                InstructorId = userId
            };

            context.Courses.Add(course);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Instructor")]
        public async Task<ActionResult<Course>> UpdateCourse(int id, CourseDto courseDto)
        {
            var course = await context.Courses.FindAsync(id);   
            if (course == null)
                return NotFound("Course not found");
            
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            if (course.InstructorId != userId && role != "Admin")
                return  StatusCode(403, "You are not allowed to update this course");
            

            course.Title = courseDto.Title;
            course.Description = courseDto.Description;
            course.Price = courseDto.Price;
            course.ThumbnailUrl = courseDto.ThumbnailUrl;
            course.Language = courseDto.Language;
            course.CategoryId = courseDto.CategoryId;
            course.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Ok(course);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Instructor")]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            var course = await context.Courses.FindAsync(id);
            if (course == null) 
                return NotFound("Course not found");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            if (course.InstructorId != userId && role != "Admin")
                return StatusCode(403, "You are not allowed to delete this course");
            
            context.Courses.Remove(course);
            await context.SaveChangesAsync();

            return Ok("Course deleted successfully");
        }
    }
}
