using Authy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authy.Data;

public class AuthyDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public AuthyDbContext(DbContextOptions<AuthyDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Salt).IsRequired();
        });
    }
}