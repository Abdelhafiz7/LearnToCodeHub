using System;
using API.Enums;

namespace API.Entities;

public class User
{
    public int Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public UserRole? Role { get; set; } = UserRole.Student;

    public string? Bio { get; set; }

    public string? ProfileImageUrl { get; set; }

    public bool isActive { get; set; } = true;

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;


    //Navigation properties
    public ICollection<Course> CoursesCreated { get; set; } = new List<Course>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

}
