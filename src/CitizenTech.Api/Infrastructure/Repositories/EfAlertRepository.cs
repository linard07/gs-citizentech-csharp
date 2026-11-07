using CitizenTech.Api.Application.Abstractions;
using CitizenTech.Api.Domain;
using CitizenTech.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CitizenTech.Api.Infrastructure.Repositories;

public class EfAlertRepository : IAlertRepository
{
    private readonly AppDbContext _db;

    public EfAlertRepository(AppDbContext db) => _db = db;

    public Task<WeatherAlert?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _db.Alerts.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<WeatherAlert> Items, int TotalCount)> SearchAsync(
        AlertType? type, AlertStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        IQueryable<WeatherAlert> query = _db.Alerts.AsNoTracking();

        if (type.HasValue)
            query = query.Where(a => a.Type == type.Value);
        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.StartedAtUtc)
            .ThenByDescending(a => a.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<AlertSummary>> GetSummariesAsync(CancellationToken cancellationToken = default)
        => await _db.Alerts
            .AsNoTracking()
            .Select(a => new AlertSummary(a.Type, a.Status, a.CheckIns.Sum(c => c.People)))
            .ToListAsync(cancellationToken);

    public async Task AddAsync(WeatherAlert alert, CancellationToken cancellationToken = default)
        => await _db.Alerts.AddAsync(alert, cancellationToken);
}
