using System.ComponentModel.DataAnnotations;
using CitizenTech.Api.Domain;

namespace CitizenTech.Api.Application.Dtos;

/// <summary>Medição que será avaliada para decidir se um alerta deve ser criado.</summary>
public sealed class CreateAlertRequest
{
    /// <summary>Tipo de evento: ChuvaForte, Alagamento, CalorExtremo ou FrioExtremo.</summary>
    [EnumDataType(typeof(AlertType))]
    public AlertType Type { get; init; }

    /// <summary>
    /// Valor medido. Unidade por tipo: ChuvaForte = mm/dia, Alagamento = cm de água,
    /// CalorExtremo = °C (máxima), FrioExtremo = °C (mínima).
    /// </summary>
    public double MeasuredValue { get; init; }

    [Required, StringLength(120)]
    public string Region { get; init; } = string.Empty;

    [Range(-90, 90)]
    public double Latitude { get; init; }

    [Range(-180, 180)]
    public double Longitude { get; init; }

    [StringLength(500)]
    public string? Description { get; init; }

    /// <summary>Origem do dado (ex.: INMET, Modelo ML). Padrão: Manual.</summary>
    [StringLength(60)]
    public string? Source { get; init; }

    /// <summary>Probabilidade (0 a 1) prevista pelo modelo de ML, se houver.</summary>
    [Range(0, 1)]
    public double? PredictedProbability { get; init; }
}

public sealed record AlertResponse(
    int Id,
    AlertType Type,
    AlertSeverity Severity,
    AlertStatus Status,
    double MeasuredValue,
    string Region,
    double Latitude,
    double Longitude,
    string Description,
    string Source,
    double? PredictedProbability,
    DateTime StartedAtUtc,
    DateTime? EndedAtUtc);

/// <summary>Resultado da avaliação de uma medição.</summary>
public sealed record AlertEvaluationResult(
    bool AlertCreated,
    AlertSeverity? Severity,
    string Message,
    AlertResponse? Alert);

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public sealed class CheckInRequest
{
    [Range(1, int.MaxValue)]
    public int SafeZoneId { get; init; }

    [Range(1, 100000)]
    public int People { get; init; }
}

public sealed record CheckInResponse(
    int Id,
    int AlertId,
    int SafeZoneId,
    string SafeZoneName,
    int People,
    DateTime CheckedInAtUtc,
    int SafeZoneAvailableSpots);

public sealed record AlertTypeStatistics(AlertType Type, int TotalAlerts, int ActiveAlerts, int PeopleSheltered);

public sealed record AlertStatisticsResponse(
    int TotalAlerts,
    int ActiveAlerts,
    int TotalPeopleSheltered,
    IReadOnlyList<AlertTypeStatistics> ByType);
