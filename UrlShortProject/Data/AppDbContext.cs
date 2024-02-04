using Microsoft.EntityFrameworkCore;
using UrlShortProject.Models;

namespace UrlShortProject.Data;

public class AppDbContext : DbContext
{
    public virtual DbSet<ShortUrl> Urls { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
}