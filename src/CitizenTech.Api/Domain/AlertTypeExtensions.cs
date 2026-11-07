namespace CitizenTech.Api.Domain;

public static class AlertTypeExtensions
{
    /// <summary>Serviço que um ponto de segurança precisa oferecer para atender o tipo de alerta.</summary>
    public static SafeZoneServices RequiredService(this AlertType type) => type switch
    {
        AlertType.ChuvaForte or AlertType.Alagamento => SafeZoneServices.FloodSafeShelter,
        AlertType.CalorExtremo => SafeZoneServices.AirConditioning,
        AlertType.FrioExtremo => SafeZoneServices.Heating,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Tipo de alerta desconhecido.")
    };
}
