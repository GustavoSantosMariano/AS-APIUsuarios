using APIUsuarios.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace APIUsuarios.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}