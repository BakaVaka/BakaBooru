using Microsoft.EntityFrameworkCore;
using Server.Data.Models;

namespace Server.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Picture> Pictures { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}