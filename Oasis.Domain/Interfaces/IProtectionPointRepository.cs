
using Oasis.Domain.Models;

namespace Oasis.Domain.Interfaces;

public interface IProtectionPointRepository
{
    Task<IEnumerable<ProtectionPoint>> GetAllAsync();

    Task<ProtectionPoint?> GetByIdAsync(Guid id);

    Task<ProtectionPoint> AddAsync(ProtectionPoint point);

    Task<bool> UpdateAsync(ProtectionPoint point);

    Task<bool> DeleteAsync(Guid id);
}
