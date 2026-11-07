using CitizenTech.Api.Application.Abstractions;
using CitizenTech.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace CitizenTech.Api.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<SafeZone> SafeZones => Set<SafeZone>();
    public DbSet<WeatherAlert> Alerts => Set<WeatherAlert>();
    public DbSet<ShelterCheckIn> CheckIns => Set<ShelterCheckIn>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
