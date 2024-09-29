using Microsoft.AspNetCore.Mvc;

namespace Server.Data.Models;

public class Picture
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Hash { get; set; }
    public string Description { get; set; }
    public string Extension { get; set; }
    public string Path { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public IEnumerable<Tag> Tags { get; set; }
}
