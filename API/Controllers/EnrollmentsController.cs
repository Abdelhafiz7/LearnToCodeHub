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
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController(WebContext context) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> Enroll(EnrollmentDto enrollmentDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var courseExists = await context.Courses.AnyAsync(c => c.Id == enrollmentDto.CourseId);
            if (!courseExists)
                return NotFound("Course not found");

            var exists = await context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == enrollmentDto.CourseId);

            if (exists)
                return BadRequest("Alrady enrolled in this course");

            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = enrollmentDto.CourseId,
                Status = Enums.EnrollmentStatus.Active,
            };

            context.Enrollments.Add(enrollment);
            await context.SaveChangesAsync();

            return Ok("Enrolled successfully");
        }

        [HttpGet("my-courses")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<List<EnrollmentDto>>> GetMyCourses()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var enrollments = await context.Enrollments
                .Where(e => e.UserId == userId)
                .Include(e => e.Course)
                .Select(e => new EnrollmentDto
                {
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    Status = e.Status.ToString(),
                    EnrolledAt = e.EnrolledAt,
                    CompletedAt = e.CompletedAt ?? DateTime.MinValue
                }).ToListAsync();

            return Ok(enrollments);
        }

        [HttpPut("complete/{courseId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> CompleteCourse(int courseId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var enrollment = await context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (enrollment == null)
                return NotFound("Not enrolled in this course");

            enrollment.Status = Enums.EnrollmentStatus.Completed;
            enrollment.CompletedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Ok("Course marked as completed");
        }

        [HttpDelete("{courseId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> CancelEnrollment(int courseId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var enrollment = await context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (enrollment == null)
                return NotFound("Enrollment not found");

            context.Enrollments.Remove(enrollment);
            await context.SaveChangesAsync();

            return Ok("Enrollment cancelled successfully");
        }

    }
}
