using CitizenTech.Api.Domain;

namespace CitizenTech.Tests;

public class SafeZoneTests
{
    private static SafeZone NewZone(int capacity = 100, SafeZoneServices services = SafeZoneServices.AirConditioning)
        => new("Ponto Teste", "Rua Teste, 1", -23.55, -46.63, capacity, services);

    [Fact]
    public void Constructor_SetsInitialState()
    {
        var zone = NewZone(100);

        Assert.Equal(100, zone.AvailableSpots);
        Assert.Equal(0, zone.CurrentOccupancy);
        Assert.True(zone.IsActive);
        Assert.True(zone.IsFictitious);
    }

    [Theory]
    [InlineData("", "Rua", 10)]
    [InlineData("Nome", " ", 10)]
    [InlineData("Nome", "Rua", 0)]
    public void Constructor_RejectsInvalidData(string name, string address, int capacity)
        => Assert.Throws<DomainException>(() => new SafeZone(name, address, -23.5, -46.6, capacity, SafeZoneServices.None));

    [Fact]
    public void CheckIn_IncreasesOccupancy()
    {
        var zone = NewZone(100);

        zone.CheckIn(30);

        Assert.Equal(30, zone.CurrentOccupancy);
        Assert.Equal(70, zone.AvailableSpots);
    }

    [Fact]
    public void CheckIn_BeyondCapacity_Throws()
    {
        var zone = NewZone(10);

        Assert.Throws<DomainException>(() => zone.CheckIn(11));
        Assert.Equal(0, zone.CurrentOccupancy);
    }

    [Fact]
    public void CheckIn_OnInactiveZone_Throws()
    {
        var zone = NewZone();
        zone.Deactivate();

        Assert.Throws<DomainException>(() => zone.CheckIn(1));
    }

    [Fact]
    public void CheckOut_ReleasesSpots_AndCannotGoBelowZero()
    {
        var zone = NewZone(50);
        zone.CheckIn(20);

        zone.CheckOut(15);

        Assert.Equal(5, zone.CurrentOccupancy);
        Assert.Throws<DomainException>(() => zone.CheckOut(6));
    }

    [Fact]
    public void Update_CannotReduceCapacityBelowCurrentOccupancy()
    {
        var zone = NewZone(50);
        zone.CheckIn(40);

        Assert.Throws<DomainException>(() => zone.Update("Novo", "Rua Nova", -23.5, -46.6, 30, SafeZoneServices.Heating));
    }

    [Theory]
    [InlineData(AlertType.CalorExtremo, true)]
    [InlineData(AlertType.FrioExtremo, false)]
    [InlineData(AlertType.ChuvaForte, false)]
    public void Supports_DependsOnOfferedServices(AlertType type, bool expected)
        => Assert.Equal(expected, NewZone(100, SafeZoneServices.AirConditioning).Supports(type));
}
