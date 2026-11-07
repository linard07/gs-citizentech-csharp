using CitizenTech.Api.Application.Abstractions;
using CitizenTech.Api.Domain;
using CitizenTech.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CitizenTech.Api.Infrastructure.Repositories;

public class EfSafeZoneRepository : ISafeZoneRepository
{
    private readonly AppDbContext _db;

    public EfSafeZoneRepository(AppDbContext db) => _db = db;

    public Task<SafeZone?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _db.SafeZones.FirstOrDefaultAsync(z => z.Id == id, cancellationToken);

    public async Task<IReadOnlyList<SafeZone>> ListAsync(bool onlyActive, CancellationToken cancellationToken = default)
    {
        IQueryable<SafeZone> query = _db.SafeZones.AsNoTracking();
        if (onlyActive)
            query = query.Where(z => z.IsActive);

        return await query.OrderBy(z => z.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SafeZone>> ListWithAvailabilityAsync(int minSpots, CancellationToken cancellationToken = default)
        => await _db.SafeZones
            .AsNoTracking()
            .Where(z => z.IsActive && z.Capacity - z.CurrentOccupancy >= minSpots)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(SafeZone zone, CancellationToken cancellationToken = default)
        => await _db.SafeZones.AddAsync(zone, cancellationToken);
}
