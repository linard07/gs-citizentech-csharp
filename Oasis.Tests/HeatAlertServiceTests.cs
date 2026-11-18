
using Oasis.Api.Services;
using Xunit;

namespace Oasis.Tests;

public class HeatAlertServiceTests
{
    private readonly HeatAlertService _service = new();

    [Theory]
    [InlineData(25, "LOW")]
    [InlineData(30, "MODERATE")]
    [InlineData(34.9, "MODERATE")]
    [InlineData(35, "HIGH")]
    [InlineData(39.9, "HIGH")]
    [InlineData(40, "EXTREME")]
    [InlineData(45, "EXTREME")]
    public void Analyze_ShouldReturnCorrectRiskLevel(
        double temperature,
        string expectedRisk)
    {
        var result = _service.Analyze(temperature);

        Assert.Equal(expectedRisk, result.RiskLevel);
        Assert.Equal(temperature, result.TemperatureC);
        Assert.True(result.IsSimulation);
    }

    [Fact]
    public void Analyze_ShouldReturnRecommendations()
    {
        var result = _service.Analyze(35);

        Assert.NotEmpty(result.Recommendations);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Analyze_ShouldRecommendEmergencyCareForExtremeHeat()
    {
        var result = _service.Analyze(42);

        Assert.Contains(
            result.Recommendations,
            recommendation =>
                recommendation.Contains("emergência"));
    }
}
