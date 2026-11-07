using CitizenTech.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CitizenTech.Api.Infrastructure.Persistence.Configurations;

public class SafeZoneConfiguration : IEntityTypeConfiguration<SafeZone>
{
    public void Configure(EntityTypeBuilder<SafeZone> builder)
    {
        builder.ToTable("SafeZones");
        builder.HasKey(z => z.Id);
        builder.Property(z => z.Name).IsRequired().HasMaxLength(120);
        builder.Property(z => z.Address).IsRequired().HasMaxLength(200);
        builder.Property(z => z.Services).HasConversion<int>();
        builder.HasIndex(z => z.IsActive);
    }
}

public class WeatherAlertConfiguration : IEntityTypeConfiguration<WeatherAlert>
{
    public void Configure(EntityTypeBuilder<WeatherAlert> builder)
    {
        builder.ToTable("WeatherAlerts");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Type).HasConversion<string>().HasMaxLength(30);
        builder.Property(a => a.Severity).HasConversion<string>().HasMaxLength(30);
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(a => a.Region).IsRequired().HasMaxLength(120);
        builder.Property(a => a.Description).IsRequired().HasMaxLength(500);
        builder.Property(a => a.Source).IsRequired().HasMaxLength(60);

        builder.HasMany(a => a.CheckIns)
            .WithOne(c => c.Alert)
            .HasForeignKey(c => c.WeatherAlertId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(a => a.CheckIns).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(a => new { a.Type, a.Status });
    }
}

public class ShelterCheckInConfiguration : IEntityTypeConfiguration<ShelterCheckIn>
{
    public void Configure(EntityTypeBuilder<ShelterCheckIn> builder)
    {
        builder.ToTable("ShelterCheckIns");
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.SafeZone)
            .WithMany()
            .HasForeignKey(c => c.SafeZoneId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
