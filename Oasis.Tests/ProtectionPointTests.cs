
using Oasis.Domain.Models;

namespace Oasis.Tests;

public class ProtectionPointTests
{
    [Fact]
    public void NewPoint_ShouldGenerateUniqueId()
    {
        var point = new ProtectionPoint();

        Assert.NotEqual(Guid.Empty, point.Id);
    }

    [Fact]
    public void NewPoint_ShouldBeActiveByDefault()
    {
        var point = new ProtectionPoint();

        Assert.True(point.IsActive);
    }

    [Fact]
    public void NewPoint_ShouldHaveCreationDate()
    {
        var before = DateTime.UtcNow;

        var point = new ProtectionPoint();

        var after = DateTime.UtcNow;

        Assert.InRange(point.CreatedAt, before, after);
    }

    [Fact]
    public void ProtectionType_ShouldContainThreeTypes()
    {
        var types = Enum.GetValues<ProtectionType>();

        Assert.Equal(3, types.Length);
    }

    [Fact]
    public void Point_ShouldStoreLocationData()
    {
        var point = new ProtectionPoint
        {
            Name = "Oasis República",
            Address = "Praça da República, São Paulo",
            Latitude = -23.5432,
            Longitude = -46.6427,
            Type = ProtectionType.FreeWater
        };

        Assert.Equal("Oasis República", point.Name);
        Assert.Equal(-23.5432, point.Latitude);
        Assert.Equal(-46.6427, point.Longitude);
        Assert.Equal(ProtectionType.FreeWater, point.Type);
    }
}
