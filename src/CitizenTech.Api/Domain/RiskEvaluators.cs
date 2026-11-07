namespace CitizenTech.Api.Domain;

/// <summary>Estratégia que converte uma medição em severidade de alerta (ou nenhuma).</summary>
public interface IRiskEvaluator
{
    AlertType Type { get; }
    string Unit { get; }

    /// <returns>A severidade, ou <c>null</c> se o valor estiver abaixo do limiar de alerta.</returns>
    AlertSeverity? Evaluate(double measuredValue);
}

/// <summary>Avaliador baseado em três limiares (atenção, alerta e emergência).</summary>
public abstract class ThresholdRiskEvaluator : IRiskEvaluator
{
    private readonly double _attention;
    private readonly double _warning;
    private readonly double _emergency;
    private readonly bool _lowerIsWorse;

    protected ThresholdRiskEvaluator(AlertType type, string unit, double attention, double warning,
        double emergency, bool lowerIsWorse = false)
    {
        Type = type;
        Unit = unit;
        _attention = attention;
        _warning = warning;
        _emergency = emergency;
        _lowerIsWorse = lowerIsWorse;
    }

    public AlertType Type { get; }
    public string Unit { get; }

    public AlertSeverity? Evaluate(double measuredValue)
    {
        if (Reached(measuredValue, _emergency)) return AlertSeverity.Emergencia;
        if (Reached(measuredValue, _warning)) return AlertSeverity.Alerta;
        if (Reached(measuredValue, _attention)) return AlertSeverity.Atencao;
        return null;
    }

    private bool Reached(double value, double threshold)
        => _lowerIsWorse ? value <= threshold : value >= threshold;
}

/// <summary>Chuva acumulada no dia. O limiar de 30 mm é o mesmo do modelo de ML do projeto.</summary>
public sealed class RainRiskEvaluator : ThresholdRiskEvaluator
{
    public RainRiskEvaluator() : base(AlertType.ChuvaForte, "mm/dia", 30, 50, 80) { }
}

/// <summary>Nível da água em via pública, em centímetros.</summary>
public sealed class FloodRiskEvaluator : ThresholdRiskEvaluator
{
    public FloodRiskEvaluator() : base(AlertType.Alagamento, "cm", 10, 30, 60) { }
}

/// <summary>Temperatura máxima do dia, em °C.</summary>
public sealed class HeatRiskEvaluator : ThresholdRiskEvaluator
{
    public HeatRiskEvaluator() : base(AlertType.CalorExtremo, "°C", 35, 38, 40) { }
}

/// <summary>Temperatura mínima do dia, em °C (quanto menor, pior).</summary>
public sealed class ColdRiskEvaluator : ThresholdRiskEvaluator
{
    public ColdRiskEvaluator() : base(AlertType.FrioExtremo, "°C", 8, 5, 2, lowerIsWorse: true) { }
}
