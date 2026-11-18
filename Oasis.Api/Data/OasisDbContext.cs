
using Microsoft.EntityFrameworkCore;
using Oasis.Domain.Models;

namespace Oasis.Api.Data;

public class OasisDbContext : DbContext
{
    public OasisDbContext(DbContextOptions<OasisDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProtectionPoint> ProtectionPoints => Set<ProtectionPoint>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProtectionPoint>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(p => p.Description)
                .HasMaxLength(500);

            entity.Property(p => p.Address)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(p => p.Type)
                .HasConversion<string>()
                .HasMaxLength(30);
        });
    }
}
