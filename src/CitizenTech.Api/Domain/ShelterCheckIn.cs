namespace CitizenTech.Api.Domain;

/// <summary>Registro de entrada de pessoas em um ponto de segurança durante um alerta.</summary>
public class ShelterCheckIn : Entity
{
    private ShelterCheckIn() // usado pelo EF Core
    {
        Alert = null!;
        SafeZone = null!;
    }

    internal ShelterCheckIn(WeatherAlert alert, SafeZone safeZone, int people)
    {
        Alert = alert;
        SafeZone = safeZone;
        WeatherAlertId = alert.Id;
        SafeZoneId = safeZone.Id;
        People = people;
        CheckedInAtUtc = DateTime.UtcNow;
    }

    public int WeatherAlertId { get; private set; }
    public int SafeZoneId { get; private set; }
    public int People { get; private set; }
    public DateTime CheckedInAtUtc { get; private set; }

    public WeatherAlert Alert { get; private set; }
    public SafeZone SafeZone { get; private set; }
}
