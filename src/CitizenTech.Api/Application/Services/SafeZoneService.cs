using CitizenTech.Api.Application.Abstractions;
using CitizenTech.Api.Application.Dtos;
using CitizenTech.Api.Domain;

namespace CitizenTech.Api.Application.Services;

public interface ISafeZoneService
{
    Task<IReadOnlyList<SafeZoneResponse>> ListAsync(bool onlyActive, CancellationToken cancellationToken = default);
    Task<SafeZoneResponse> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<SafeZoneResponse> CreateAsync(SafeZoneRequest request, CancellationToken cancellationToken = default);
    Task<SafeZoneResponse> UpdateAsync(int id, SafeZoneRequest request, CancellationToken cancellationToken = default);
    Task DeactivateAsync(int id, CancellationToken cancellationToken = default);
    Task<SafeZoneResponse> CheckOutAsync(int id, CheckOutRequest request, CancellationToken cancellationToken = default);
}

public class SafeZoneService : ISafeZoneService
{
    private readonly ISafeZoneRepository _zones;
    private readonly IUnitOfWork _unitOfWork;

    public SafeZoneService(ISafeZoneRepository zones, IUnitOfWork unitOfWork)
    {
        _zones = zones;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<SafeZoneResponse>> ListAsync(bool onlyActive, CancellationToken cancellationToken = default)
        => (await _zones.ListAsync(onlyActive, cancellationToken)).Select(z => z.ToResponse()).ToList();

    public async Task<SafeZoneResponse> GetAsync(int id, CancellationToken cancellationToken = default)
        => (await GetOrThrowAsync(id, cancellationToken)).ToResponse();

    public async Task<SafeZoneResponse> CreateAsync(SafeZoneRequest request, CancellationToken cancellationToken = default)
    {
        var zone = new SafeZone(request.Name, request.Address, request.Latitude, request.Longitude,
            request.Capacity, request.Services);

        await _zones.AddAsync(zone, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return zone.ToResponse();
    }

    public async Task<SafeZoneResponse> UpdateAsync(int id, SafeZoneRequest request, CancellationToken cancellationToken = default)
    {
        var zone = await GetOrThrowAsync(id, cancellationToken);
        zone.Update(request.Name, request.Address, request.Latitude, request.Longitude, request.Capacity, request.Services);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return zone.ToResponse();
    }

    public async Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var zone = await GetOrThrowAsync(id, cancellationToken);
        zone.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<SafeZoneResponse> CheckOutAsync(int id, CheckOutRequest request, CancellationToken cancellationToken = default)
    {
        var zone = await GetOrThrowAsync(id, cancellationToken);
        zone.CheckOut(request.People);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return zone.ToResponse();
    }

    private async Task<SafeZone> GetOrThrowAsync(int id, CancellationToken cancellationToken)
        => await _zones.GetByIdAsync(id, cancellationToken)
           ?? throw new NotFoundException($"Ponto de segurança {id} não encontrado.");
}
