using CitizenTech.Api.Domain;

namespace CitizenTech.Tests;

public class GeoDistanceTests
{
    [Fact]
    public void SamePoint_HasZeroDistance()
        => Assert.Equal(0.0, GeoDistance.HaversineKm(-23.55, -46.63, -23.55, -46.63), 6);

    [Fact]
    public void Se_To_Pinheiros_IsAboutSevenKilometers()
    {
        var km = GeoDistance.HaversineKm(-23.5503, -46.6339, -23.5670, -46.7020);

        Assert.InRange(km, 6.5, 8.0);
    }

    [Fact]
    public void Distance_IsSymmetric()
    {
        var ab = GeoDistance.HaversineKm(-23.55, -46.63, -23.65, -46.70);
        var ba = GeoDistance.HaversineKm(-23.65, -46.70, -23.55, -46.63);

        Assert.Equal(ab, ba, 9);
    }
}
