
namespace Oasis.Api.Services;

public static class GeoDistanceService
{
    private const double EarthRadiusKm = 6371.0;

    public static double CalculateKm(
        double latitude1,
        double longitude1,
        double latitude2,
        double longitude2)
    {
        double ToRadians(double degrees) =>
            degrees * Math.PI / 180.0;

        var latDifference =
            ToRadians(latitude2 - latitude1);

        var lonDifference =
            ToRadians(longitude2 - longitude1);

        var a =
            Math.Pow(Math.Sin(latDifference / 2), 2) +
            Math.Cos(ToRadians(latitude1)) *
            Math.Cos(ToRadians(latitude2)) *
            Math.Pow(Math.Sin(lonDifference / 2), 2);

        var c = 2 * Math.Atan2(
            Math.Sqrt(a),
            Math.Sqrt(1 - a)
        );

        return EarthRadiusKm * c;
    }
}
