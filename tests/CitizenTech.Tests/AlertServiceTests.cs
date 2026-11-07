using CitizenTech.Api.Application;
using CitizenTech.Api.Application.Dtos;
using CitizenTech.Api.Application.Services;
using CitizenTech.Api.Domain;
using CitizenTech.Api.Infrastructure.Persistence;
using CitizenTech.Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace CitizenTech.Tests;

public class AlertServiceTests
{
    private static (AlertService Service, AppDbContext Db) CreateSut()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new AppDbContext(options);
        var evaluators = new IRiskEvaluator[]
        {
            new RainRiskEvaluator(), new FloodRiskEvaluator(), new HeatRiskEvaluator(), new ColdRiskEvaluator()
        };

        var service = new AlertService(new EfAlertRepository(db), new EfSafeZoneRepository(db), db,
            evaluators, NullLogger<AlertService>.Instance);

        return (service, db);
    }

    private static CreateAlertRequest Request(AlertType type, double value) => new()
    {
        Type = type,
        MeasuredValue = value,
        Region = "Sé",
        Latitude = -23.5503,
        Longitude = -46.6339
    };

    private static async Task<SafeZone> AddZoneAsync(AppDbContext db, string name, double lat, double lon,
        int capacity, SafeZoneServices services)
    {
        var zone = new SafeZone(name, "Endereço fictício", lat, lon, capacity, services);
        db.SafeZones.Add(zone);
        await db.SaveChangesAsync();
        return zone;
    }

    [Fact]
    public async Task EvaluateAndCreate_AboveThreshold_PersistsAlert()
    {
        var (service, db) = CreateSut();

        var result = await service.EvaluateAndCreateAsync(Request(AlertType.ChuvaForte, 55));

        Assert.True(result.AlertCreated);
        Assert.Equal(AlertSeverity.Alerta, result.Severity);
        Assert.Equal(1, await db.Alerts.CountAsync());
    }

    [Fact]
    public async Task EvaluateAndCreate_BelowThreshold_DoesNotCreateAlert()
    {
        var (service, db) = CreateSut();

        var result = await service.EvaluateAndCreateAsync(Request(AlertType.ChuvaForte, 10));

        Assert.False(result.AlertCreated);
        Assert.Null(result.Alert);
        Assert.Equal(0, await db.Alerts.CountAsync());
    }

    [Fact]
    public async Task GetAsync_UnknownId_ThrowsNotFound()
    {
        var (service, _) = CreateSut();

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetAsync(999));
    }

    [Fact]
    public async Task Close_Twice_ThrowsDomainException()
    {
        var (service, _) = CreateSut();
        var created = await service.EvaluateAndCreateAsync(Request(AlertType.ChuvaForte, 60));
        var id = created.Alert!.Id;

        var closed = await service.CloseAsync(id);

        Assert.Equal(AlertStatus.Encerrado, closed.Status);
        await Assert.ThrowsAsync<DomainException>(() => service.CloseAsync(id));
    }

    [Fact]
    public async Task GetNearestSafeZones_FiltersByTypeAndAvailability_AndOrdersByDistance()
    {
        var (service, db) = CreateSut();
        await AddZoneAsync(db, "Longe (enchente)", -23.60, -46.70, 50, SafeZoneServices.FloodSafeShelter);
        await AddZoneAsync(db, "Perto (enchente)", -23.551, -46.634, 50, SafeZoneServices.FloodSafeShelter);
        await AddZoneAsync(db, "Perto (só ar-condicionado)", -23.5505, -46.634, 50, SafeZoneServices.AirConditioning);
        var full = await AddZoneAsync(db, "Perto (lotado)", -23.5506, -46.634, 1, SafeZoneServices.FloodSafeShelter);
        full.CheckIn(1);
        await db.SaveChangesAsync();

        var created = await service.EvaluateAndCreateAsync(Request(AlertType.ChuvaForte, 60));

        var nearest = await service.GetNearestSafeZonesAsync(created.Alert!.Id, null, null, 5);

        Assert.Equal(2, nearest.Count);
        Assert.Equal("Perto (enchente)", nearest[0].SafeZone.Name);
        Assert.Equal("Longe (enchente)", nearest[1].SafeZone.Name);
        Assert.True(nearest[0].DistanceKm < nearest[1].DistanceKm);
    }

    [Fact]
    public async Task RegisterCheckIn_UpdatesOccupancyAndPersistsRecord()
    {
        var (service, db) = CreateSut();
        var zone = await AddZoneAsync(db, "Abrigo", -23.55, -46.63, 100, SafeZoneServices.FloodSafeShelter);
        var created = await service.EvaluateAndCreateAsync(Request(AlertType.Alagamento, 35));

        var response = await service.RegisterCheckInAsync(created.Alert!.Id, new CheckInRequest { SafeZoneId = zone.Id, People = 40 });

        Assert.Equal(40, response.People);
        Assert.Equal(60, response.SafeZoneAvailableSpots);
        Assert.Equal(1, await db.CheckIns.CountAsync());
    }

    [Fact]
    public async Task RegisterCheckIn_UnknownAlert_ThrowsNotFound()
    {
        var (service, db) = CreateSut();
        var zone = await AddZoneAsync(db, "Abrigo", -23.55, -46.63, 100, SafeZoneServices.FloodSafeShelter);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.RegisterCheckInAsync(123, new CheckInRequest { SafeZoneId = zone.Id, People = 1 }));
    }

    [Fact]
    public async Task Statistics_GroupsByTypeAndSumsPeopleSheltered()
    {
        var (service, db) = CreateSut();
        var zone = await AddZoneAsync(db, "Abrigo", -23.55, -46.63, 100, SafeZoneServices.FloodSafeShelter);
        var rain = await service.EvaluateAndCreateAsync(Request(AlertType.ChuvaForte, 60));
        await service.EvaluateAndCreateAsync(Request(AlertType.CalorExtremo, 39));
        await service.RegisterCheckInAsync(rain.Alert!.Id, new CheckInRequest { SafeZoneId = zone.Id, People = 10 });

        var stats = await service.GetStatisticsAsync();

        Assert.Equal(2, stats.TotalAlerts);
        Assert.Equal(2, stats.ActiveAlerts);
        Assert.Equal(10, stats.TotalPeopleSheltered);
        Assert.Equal(10, stats.ByType.Single(s => s.Type == AlertType.ChuvaForte).PeopleSheltered);
    }

    [Fact]
    public async Task Search_FiltersByType()
    {
        var (service, _) = CreateSut();
        await service.EvaluateAndCreateAsync(Request(AlertType.ChuvaForte, 60));
        await service.EvaluateAndCreateAsync(Request(AlertType.CalorExtremo, 39));

        var page = await service.SearchAsync(AlertType.CalorExtremo, null, 1, 10);

        Assert.Equal(1, page.TotalCount);
        Assert.Equal(AlertType.CalorExtremo, page.Items[0].Type);
    }
}
