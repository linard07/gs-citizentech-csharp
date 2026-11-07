using CitizenTech.Api.Domain;

namespace CitizenTech.Tests;

public class WeatherAlertTests
{
    private static WeatherAlert NewAlert(AlertType type = AlertType.ChuvaForte)
        => new(type, AlertSeverity.Alerta, 55, "Zona Leste", -23.55, -46.63, null, null);

    private static SafeZone FloodZone(int capacity = 100)
        => new("Abrigo", "Rua A", -23.55, -46.63, capacity, SafeZoneServices.FloodSafeShelter);

    [Fact]
    public void Constructor_AppliesDefaults()
    {
        var alert = NewAlert();

        Assert.Equal(AlertStatus.Ativo, alert.Status);
        Assert.Equal("Manual", alert.Source);
        Assert.False(string.IsNullOrWhiteSpace(alert.Description));
        Assert.Null(alert.EndedAtUtc);
    }

    [Fact]
    public void Constructor_RejectsEmptyRegion()
        => Assert.Throws<DomainException>(() =>
            new WeatherAlert(AlertType.ChuvaForte, AlertSeverity.Atencao, 31, " ", -23.5, -46.6, null, null));

    [Fact]
    public void Constructor_RejectsInvalidProbability()
        => Assert.Throws<DomainException>(() =>
            new WeatherAlert(AlertType.ChuvaForte, AlertSeverity.Atencao, 31, "Centro", -23.5, -46.6, null, null, 1.5));

    [Fact]
    public void Close_SetsStatusAndEndDate_AndCannotBeRepeated()
    {
        var alert = NewAlert();

        alert.Close();

        Assert.Equal(AlertStatus.Encerrado, alert.Status);
        Assert.NotNull(alert.EndedAtUtc);
        Assert.Throws<DomainException>(() => alert.Close());
    }

    [Fact]
    public void RegisterCheckIn_UpdatesZoneOccupancy()
    {
        var alert = NewAlert();
        var zone = FloodZone(100);

        var checkIn = alert.RegisterCheckIn(zone, 25);

        Assert.Equal(25, checkIn.People);
        Assert.Equal(25, zone.CurrentOccupancy);
        Assert.Single(alert.CheckIns);
    }

    [Fact]
    public void RegisterCheckIn_OnClosedAlert_Throws()
    {
        var alert = NewAlert();
        alert.Close();

        Assert.Throws<DomainException>(() => alert.RegisterCheckIn(FloodZone(), 1));
    }

    [Fact]
    public void RegisterCheckIn_InIncompatibleZone_Throws()
    {
        var heatAlert = NewAlert(AlertType.CalorExtremo);

        Assert.Throws<DomainException>(() => heatAlert.RegisterCheckIn(FloodZone(), 1));
    }
}
