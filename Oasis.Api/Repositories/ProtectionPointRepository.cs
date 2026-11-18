
using Microsoft.EntityFrameworkCore;
using Oasis.Api.Data;
using Oasis.Domain.Interfaces;
using Oasis.Domain.Models;

namespace Oasis.Api.Repositories;

public class ProtectionPointRepository : IProtectionPointRepository
{
    private readonly OasisDbContext _context;

    public ProtectionPointRepository(OasisDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProtectionPoint>> GetAllAsync()
    {
        return await _context.ProtectionPoints
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<ProtectionPoint?> GetByIdAsync(Guid id)
    {
        return await _context.ProtectionPoints
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ProtectionPoint> AddAsync(ProtectionPoint point)
    {
        _context.ProtectionPoints.Add(point);
        await _context.SaveChangesAsync();

        return point;
    }

    public async Task<bool> UpdateAsync(ProtectionPoint point)
    {
        var existing = await _context.ProtectionPoints
            .FirstOrDefaultAsync(p => p.Id == point.Id);

        if (existing is null)
            return false;

        existing.Name = point.Name;
        existing.Description = point.Description;
        existing.Address = point.Address;
        existing.Latitude = point.Latitude;
        existing.Longitude = point.Longitude;
        existing.Type = point.Type;
        existing.IsActive = point.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.ProtectionPoints
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existing is null)
            return false;

        _context.ProtectionPoints.Remove(existing);
        await _context.SaveChangesAsync();

        return true;
    }
}