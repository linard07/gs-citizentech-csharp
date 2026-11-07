using CitizenTech.Api.Domain;

namespace CitizenTech.Api.Application.Abstractions;

/// <summary>Confirma (persiste) todas as alterações feitas na unidade de trabalho atual.</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

/// <summary>Resumo leve de um alerta, usado nas estatísticas.</summary>
public sealed record AlertSummary(AlertType Type, AlertStatus Status, int PeopleSheltered);

public interface IAlertRepository
{
    Task<WeatherAlert?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<WeatherAlert> Items, int TotalCount)> SearchAsync(
        AlertType? type, AlertStatus? status, int page, int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AlertSummary>> GetSummariesAsync(CancellationToken cancellationToken = default);

    Task AddAsync(WeatherAlert alert, CancellationToken cancellationToken = default);
}

public interface ISafeZoneRepository
{
    Task<SafeZone?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SafeZone>> ListAsync(bool onlyActive, CancellationToken cancellationToken = default);

    /// <summary>Pontos ativos que ainda têm pelo menos <paramref name="minSpots"/> vagas.</summary>
    Task<IReadOnlyList<SafeZone>> ListWithAvailabilityAsync(int minSpots, CancellationToken cancellationToken = default);

    Task AddAsync(SafeZone zone, CancellationToken cancellationToken = default);
}
