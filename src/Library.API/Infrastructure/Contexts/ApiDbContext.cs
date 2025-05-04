using System.Reflection;

using Library.API.Domain.Models;

using Microsoft.EntityFrameworkCore;

namespace Library.API.Infrastructure.Contexts;

public class ApiDbContext(DbContextOptions<ApiDbContext> options) : DbContext(options)
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}


