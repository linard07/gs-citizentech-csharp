
namespace Oasis.Domain.Models;

public class HeatAlert
{
    public double TemperatureC { get; set; }

    public string RiskLevel { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public List<string> Recommendations { get; set; } = new();

    public bool IsSimulation { get; set; } = true;
}
