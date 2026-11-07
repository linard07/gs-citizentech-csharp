namespace CitizenTech.Api.Domain;

/// <summary>Alerta de evento extremo (chuva forte, alagamento, calor ou frio extremos).</summary>
public class WeatherAlert : Entity
{
    private readonly List<ShelterCheckIn> _checkIns = new();

    private WeatherAlert() // usado pelo EF Core
    {
        Region = string.Empty;
        Description = string.Empty;
        Source = string.Empty;
    }

    public WeatherAlert(AlertType type, AlertSeverity severity, double measuredValue, string region,
        double latitude, double longitude, string? description, string? source, double? predictedProbability = null)
    {
        if (string.IsNullOrWhiteSpace(region))
            throw new DomainException("A região do alerta é obrigatória.");
        if (latitude is < -90 or > 90 || longitude is < -180 or > 180)
            throw new DomainException("Coordenadas inválidas.");
        if (double.IsNaN(measuredValue) || double.IsInfinity(measuredValue))
            throw new DomainException("O valor medido é inválido.");
        if (predictedProbability is < 0 or > 1)
            throw new DomainException("A probabilidade prevista deve estar entre 0 e 1.");

        Type = type;
        Severity = severity;
        MeasuredValue = measuredValue;
        Region = region.Trim();
        Latitude = latitude;
        Longitude = longitude;
        Description = string.IsNullOrWhiteSpace(description)
            ? $"Alerta de {type} com severidade {severity}."
            : description.Trim();
        Source = string.IsNullOrWhiteSpace(source) ? "Manual" : source.Trim();
        PredictedProbability = predictedProbability;
        Status = AlertStatus.Ativo;
        StartedAtUtc = DateTime.UtcNow;
    }

    public AlertType Type { get; private set; }
    public AlertSeverity Severity { get; private set; }
    public AlertStatus Status { get; private set; }
    public double MeasuredValue { get; private set; }
    public string Region { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string Description { get; private set; }
    public string Source { get; private set; }
    public double? PredictedProbability { get; private set; }
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? EndedAtUtc { get; private set; }

    public IReadOnlyCollection<ShelterCheckIn> CheckIns => _checkIns.AsReadOnly();

    public void Close()
    {
        if (Status == AlertStatus.Encerrado)
            throw new DomainException("O alerta já está encerrado.");

        Status = AlertStatus.Encerrado;
        EndedAtUtc = DateTime.UtcNow;
    }

    /// <summary>Registra a entrada de pessoas em um ponto de segurança compatível com este alerta.</summary>
    public ShelterCheckIn RegisterCheckIn(SafeZone zone, int people)
    {
        if (Status != AlertStatus.Ativo)
            throw new DomainException("Não é possível registrar entradas em um alerta encerrado.");
        if (!zone.Supports(Type))
            throw new DomainException($"O ponto \"{zone.Name}\" não atende alertas do tipo {Type}.");

        zone.CheckIn(people); // valida vagas e atualiza a ocupação

        var checkIn = new ShelterCheckIn(this, zone, people);
        _checkIns.Add(checkIn);
        return checkIn;
    }
}
