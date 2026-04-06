using API.Entities;
using API.Enums;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DbInitializer
{
    public static void InitDb(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<WebContext>()
            ?? throw new InvalidOperationException("Failed to get WebContext.");

        SeedData(context);
    }

    private static void SeedData(WebContext context)
    {
        context.Database.Migrate();

        if (context.Courses.Any()) return;

        // Create Instructor
        var instructor = new User
        {
            FirstName = "Test",
            LastName = "Instructor",
            Email = "instructor@test.com",
            PasswordHash = "test", // temp
            Role = UserRole.Instructor
        };

        context.Users.Add(instructor);

        // Create Category
        var category = new Category
        {
            Name = "Programming",
            Description = "Programming Courses"
        };

        context.Categories.Add(category);

        context.SaveChanges();

        // Create Courses
        var courses = new List<Course>
        {
            new()
            {
                Title = "Course 1",
                Description = "Description for Course 1",
                Price = 100,
                ThumbnailUrl = "https://example.com/course1.jpg",
                CategoryId = category.Id,
                InstructorId = instructor.Id,
                IsPublished = true
            },
            new()
            {
                Title = "Course 2",
                Description = "Description for Course 2",
                Price = 150,
                ThumbnailUrl = "https://example.com/course2.jpg",
                CategoryId = category.Id,
                InstructorId = instructor.Id,
                IsPublished = true
            },
            new()
            {
                Title = "Course 3",
                Description = "Description for Course 3",
                Price = 200,
                ThumbnailUrl = "https://example.com/course3.jpg",
                CategoryId = category.Id,
                InstructorId = instructor.Id,
                IsPublished = true
            }
        };

        context.Courses.AddRange(courses);
        context.SaveChanges();
    }
}