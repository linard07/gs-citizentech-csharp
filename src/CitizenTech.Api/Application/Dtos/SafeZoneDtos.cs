using System.ComponentModel.DataAnnotations;
using CitizenTech.Api.Domain;

namespace CitizenTech.Api.Application.Dtos;

public sealed class SafeZoneRequest
{
    [Required, StringLength(120)]
    public string Name { get; init; } = string.Empty;

    [Required, StringLength(200)]
    public string Address { get; init; } = string.Empty;

    [Range(-90, 90)]
    public double Latitude { get; init; }

    [Range(-180, 180)]
    public double Longitude { get; init; }

    [Range(1, 100000)]
    public int Capacity { get; init; }

    /// <summary>Serviços combináveis: AirConditioning, Heating, LowCostWater, FirstAid, FloodSafeShelter, GovernmentManaged.</summary>
    public SafeZoneServices Services { get; init; }
}

public sealed class CheckOutRequest
{
    [Range(1, 100000)]
    public int People { get; init; }
}

public sealed record SafeZoneResponse(
    int Id,
    string Name,
    string Address,
    double Latitude,
    double Longitude,
    int Capacity,
    int CurrentOccupancy,
    int AvailableSpots,
    SafeZoneServices Services,
    bool IsFictitious,
    bool IsActive);

public sealed record NearbySafeZoneResponse(SafeZoneResponse SafeZone, double DistanceKm);
