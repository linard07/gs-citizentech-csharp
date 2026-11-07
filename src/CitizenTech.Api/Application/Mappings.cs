using CitizenTech.Api.Application.Dtos;
using CitizenTech.Api.Domain;

namespace CitizenTech.Api.Application;

public static class Mappings
{
    public static AlertResponse ToResponse(this WeatherAlert a) => new(
        a.Id, a.Type, a.Severity, a.Status, a.MeasuredValue, a.Region, a.Latitude, a.Longitude,
        a.Description, a.Source, a.PredictedProbability, a.StartedAtUtc, a.EndedAtUtc);

    public static SafeZoneResponse ToResponse(this SafeZone z) => new(
        z.Id, z.Name, z.Address, z.Latitude, z.Longitude, z.Capacity, z.CurrentOccupancy,
        z.AvailableSpots, z.Services, z.IsFictitious, z.IsActive);

    public static CheckInResponse ToResponse(this ShelterCheckIn c) => new(
        c.Id, c.WeatherAlertId, c.SafeZoneId, c.SafeZone.Name, c.People, c.CheckedInAtUtc,
        c.SafeZone.AvailableSpots);
}
