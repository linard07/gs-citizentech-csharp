using CitizenTech.Api.Application.Dtos;
using CitizenTech.Api.Domain;

namespace CitizenTech.Api.Application.Services;

public interface IAlertService
{
    Task<AlertEvaluationResult> EvaluateAndCreateAsync(CreateAlertRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<AlertResponse>> SearchAsync(AlertType? type, AlertStatus? status, int page, int pageSize,
        CancellationToken cancellationToken = default);

    Task<AlertResponse> GetAsync(int id, CancellationToken cancellationToken = default);

    Task<AlertResponse> CloseAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NearbySafeZoneResponse>> GetNearestSafeZonesAsync(int alertId, double? latitude, double? longitude,
        int limit, CancellationToken cancellationToken = default);

    Task<CheckInResponse> RegisterCheckInAsync(int alertId, CheckInRequest request, CancellationToken cancellationToken = default);

    Task<AlertStatisticsResponse> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
