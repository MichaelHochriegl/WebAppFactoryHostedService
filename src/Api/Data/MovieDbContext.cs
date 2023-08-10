using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class MovieDbContext : DbContext
{
    public DbSet<Movie> Movies => Set<Movie>();

    public MovieDbContext(DbContextOptions options) : base(options)
    {
    }
}