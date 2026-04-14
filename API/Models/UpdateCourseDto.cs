using System;

namespace API.Models;

public class UpdateCourseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string Language { get; set; } = "English";
    public int CategoryId { get; set; }
}
