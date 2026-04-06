using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace API.Data;

public class WebContext : DbContext
{
    public WebContext(DbContextOptions<WebContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }   
    public DbSet<Category> Categories { get; set; } 
    public DbSet<Course> Courses { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<Lesson> Lessons {get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<LessonProgress> LessonProgresses { get; set; }
    public DbSet<Review> Reviews { get; set; }

}


