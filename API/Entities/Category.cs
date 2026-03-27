using System;

namespace API.Entities;

public class Category
{

    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    public ICollection<Course> Courses { get; set; } = new List<Course>();

}
