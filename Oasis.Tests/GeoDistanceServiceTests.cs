
using Oasis.Api.Services;
using Xunit;

namespace Oasis.Tests;

public class GeoDistanceServiceTests
{
    [Fact]
    public void SameCoordinates_ShouldReturnZero()
    {
        var distance = GeoDistanceService.CalculateKm(
            -23.5432, -46.6427,
            -23.5432, -46.6427
        );

        Assert.Equal(0, distance, 6);
    }

    [Fact]
    public void NearbyCoordinates_ShouldReturnPositiveDistance()
    {
        var distance = GeoDistanceService.CalculateKm(
            -23.5432, -46.6427,
            -23.5434, -46.6425
        );

        Assert.InRange(distance, 0.02, 0.04);
    }

    [Fact]
    public void Distance_ShouldBeSymmetric()
    {
        var distanceAB = GeoDistanceService.CalculateKm(
            -23.5432, -46.6427,
            -23.5428, -46.6424
        );

        var distanceBA = GeoDistanceService.CalculateKm(
            -23.5428, -46.6424,
            -23.5432, -46.6427
        );

        Assert.Equal(distanceAB, distanceBA, 6);
    }

    [Fact]
    public void OneDegreeLatitude_ShouldBeApproximately111Km()
    {
        var distance = GeoDistanceService.CalculateKm(
            0, 0,
            1, 0
        );

        Assert.InRange(distance, 110, 112);
    }
}
