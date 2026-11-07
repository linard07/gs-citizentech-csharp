using CitizenTech.Api.Domain;

namespace CitizenTech.Tests;

public class RiskEvaluatorTests
{
    [Theory]
    [InlineData(10.0, null)]
    [InlineData(29.9, null)]
    [InlineData(30.0, AlertSeverity.Atencao)]
    [InlineData(49.9, AlertSeverity.Atencao)]
    [InlineData(50.0, AlertSeverity.Alerta)]
    [InlineData(80.0, AlertSeverity.Emergencia)]
    [InlineData(120.0, AlertSeverity.Emergencia)]
    public void Rain_ClassifiesSeverityByThreshold(double mmPerDay, AlertSeverity? expected)
        => Assert.Equal(expected, new RainRiskEvaluator().Evaluate(mmPerDay));

    [Theory]
    [InlineData(9.0, null)]
    [InlineData(10.0, AlertSeverity.Atencao)]
    [InlineData(30.0, AlertSeverity.Alerta)]
    [InlineData(60.0, AlertSeverity.Emergencia)]
    public void Flood_ClassifiesSeverityByThreshold(double centimeters, AlertSeverity? expected)
        => Assert.Equal(expected, new FloodRiskEvaluator().Evaluate(centimeters));

    [Theory]
    [InlineData(34.9, null)]
    [InlineData(35.0, AlertSeverity.Atencao)]
    [InlineData(38.0, AlertSeverity.Alerta)]
    [InlineData(41.0, AlertSeverity.Emergencia)]
    public void Heat_ClassifiesSeverityByThreshold(double celsius, AlertSeverity? expected)
        => Assert.Equal(expected, new HeatRiskEvaluator().Evaluate(celsius));

    [Theory]
    [InlineData(12.0, null)]
    [InlineData(8.5, null)]
    [InlineData(8.0, AlertSeverity.Atencao)]
    [InlineData(5.0, AlertSeverity.Alerta)]
    [InlineData(2.0, AlertSeverity.Emergencia)]
    [InlineData(-1.0, AlertSeverity.Emergencia)]
    public void Cold_LowerValuesAreWorse(double celsius, AlertSeverity? expected)
        => Assert.Equal(expected, new ColdRiskEvaluator().Evaluate(celsius));

    [Fact]
    public void Evaluators_ExposeTheirAlertType()
    {
        IRiskEvaluator[] all = { new RainRiskEvaluator(), new FloodRiskEvaluator(), new HeatRiskEvaluator(), new ColdRiskEvaluator() };

        Assert.Equal(AlertType.ChuvaForte, all[0].Type);
        Assert.Equal(AlertType.Alagamento, all[1].Type);
        Assert.Equal(AlertType.CalorExtremo, all[2].Type);
        Assert.Equal(AlertType.FrioExtremo, all[3].Type);
    }
}
