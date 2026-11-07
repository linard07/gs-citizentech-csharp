namespace CitizenTech.Api.Domain;

/// <summary>Tipos de evento extremo monitorados pela CitizenTech.</summary>
public enum AlertType
{
    ChuvaForte = 1,
    Alagamento = 2,
    CalorExtremo = 3,
    FrioExtremo = 4
}

public enum AlertSeverity
{
    Atencao = 1,
    Alerta = 2,
    Emergencia = 3
}

public enum AlertStatus
{
    Ativo = 1,
    Encerrado = 2
}

/// <summary>Serviços oferecidos por um ponto de segurança (podem ser combinados).</summary>
[Flags]
public enum SafeZoneServices
{
    None = 0,
    AirConditioning = 1,
    Heating = 2,
    LowCostWater = 4,
    FirstAid = 8,
    FloodSafeShelter = 16,
    GovernmentManaged = 32
}
