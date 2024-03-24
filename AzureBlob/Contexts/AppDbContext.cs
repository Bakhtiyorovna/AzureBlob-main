using AzureBlob1.Entities;
using Microsoft.EntityFrameworkCore;

namespace AzureBlob1.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Admin> Admins { get; set; }
    public DbSet<Video> Videos { get; set; }
}
