using CitizenTech.Api.Application.Abstractions;
using CitizenTech.Api.Application.Dtos;
using CitizenTech.Api.Domain;
using Microsoft.Extensions.Logging;

namespace CitizenTech.Api.Application.Services;

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alerts;
    private readonly ISafeZoneRepository _safeZones;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnumerable<IRiskEvaluator> _evaluators;
    private readonly ILogger<AlertService> _logger;

    public AlertService(
        IAlertRepository alerts,
        ISafeZoneRepository safeZones,
        IUnitOfWork unitOfWork,
        IEnumerable<IRiskEvaluator> evaluators,
        ILogger<AlertService> logger)
    {
        _alerts = alerts;
        _safeZones = safeZones;
        _unitOfWork = unitOfWork;
        _evaluators = evaluators;
        _logger = logger;
    }

    public async Task<AlertEvaluationResult> EvaluateAndCreateAsync(CreateAlertRequest request, CancellationToken cancellationToken = default)
    {
        // Polimorfismo: cada tipo de evento tem o seu avaliador de risco.
        var evaluator = _evaluators.FirstOrDefault(e => e.Type == request.Type)
            ?? throw new DomainException($"Não há avaliador de risco para o tipo {request.Type}.");

        var severity = evaluator.Evaluate(request.MeasuredValue);
        if (severity is null)
        {
            return new AlertEvaluationResult(false, null,
                $"Valor {request.MeasuredValue} {evaluator.Unit} abaixo do limiar de alerta para {request.Type}. Nenhum alerta foi criado.",
                null);
        }

        var alert = new WeatherAlert(request.Type, severity.Value, request.MeasuredValue, request.Region,
            request.Latitude, request.Longitude, request.Description, request.Source, request.PredictedProbability);

        await _alerts.AddAsync(alert, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Alerta {AlertId} criado: {Type} ({Severity}) em {Region}.", alert.Id, alert.Type, alert.Severity, alert.Region);

        return new AlertEvaluationResult(true, severity, "Alerta criado.", alert.ToResponse());
    }

    public async Task<PagedResult<AlertResponse>> SearchAsync(AlertType? type, AlertStatus? status, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, total) = await _alerts.SearchAsync(type, status, page, pageSize, cancellationToken);
        return new PagedResult<AlertResponse>(items.Select(a => a.ToResponse()).ToList(), page, pageSize, total);
    }

    public async Task<AlertResponse> GetAsync(int id, CancellationToken cancellationToken = default)
        => (await GetOrThrowAsync(id, cancellationToken)).ToResponse();

    public async Task<AlertResponse> CloseAsync(int id, CancellationToken cancellationToken = default)
    {
        var alert = await GetOrThrowAsync(id, cancellationToken);
        alert.Close();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Alerta {AlertId} encerrado.", id);
        return alert.ToResponse();
    }

    public async Task<IReadOnlyList<NearbySafeZoneResponse>> GetNearestSafeZonesAsync(int alertId, double? latitude, double? longitude,
        int limit, CancellationToken cancellationToken = default)
    {
        var alert = await GetOrThrowAsync(alertId, cancellationToken);
        var originLat = latitude ?? alert.Latitude;
        var originLon = longitude ?? alert.Longitude;
        limit = Math.Clamp(limit, 1, 20);

        var candidates = await _safeZones.ListWithAvailabilityAsync(1, cancellationToken);

        // LINQ: só pontos compatíveis com o tipo de alerta, ordenados do mais próximo ao mais distante.
        return candidates
            .Where(z => z.Supports(alert.Type))
            .Select(z => new NearbySafeZoneResponse(
                z.ToResponse(),
                Math.Round(GeoDistance.HaversineKm(originLat, originLon, z.Latitude, z.Longitude), 2)))
            .OrderBy(r => r.DistanceKm)
            .Take(limit)
            .ToList();
    }

    public async Task<CheckInResponse> RegisterCheckInAsync(int alertId, CheckInRequest request, CancellationToken cancellationToken = default)
    {
        var alert = await GetOrThrowAsync(alertId, cancellationToken);
        var zone = await _safeZones.GetByIdAsync(request.SafeZoneId, cancellationToken)
            ?? throw new NotFoundException($"Ponto de segurança {request.SafeZoneId} não encontrado.");

        var checkIn = alert.RegisterCheckIn(zone, request.People);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{People} pessoa(s) registrada(s) no ponto {ZoneId} para o alerta {AlertId}.", request.People, zone.Id, alertId);
        return checkIn.ToResponse();
    }

    public async Task<AlertStatisticsResponse> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var summaries = await _alerts.GetSummariesAsync(cancellationToken);

        var byType = summaries
            .GroupBy(s => s.Type)
            .Select(g => new AlertTypeStatistics(
                g.Key,
                g.Count(),
                g.Count(s => s.Status == AlertStatus.Ativo),
                g.Sum(s => s.PeopleSheltered)))
            .OrderBy(s => s.Type)
            .ToList();

        return new AlertStatisticsResponse(
            summaries.Count,
            summaries.Count(s => s.Status == AlertStatus.Ativo),
            summaries.Sum(s => s.PeopleSheltered),
            byType);
    }

    private async Task<WeatherAlert> GetOrThrowAsync(int id, CancellationToken cancellationToken)
        => await _alerts.GetByIdAsync(id, cancellationToken)
           ?? throw new NotFoundException($"Alerta {id} não encontrado.");
}
