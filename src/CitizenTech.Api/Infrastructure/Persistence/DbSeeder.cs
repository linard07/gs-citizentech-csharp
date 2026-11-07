using CitizenTech.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace CitizenTech.Api.Infrastructure.Persistence;

/// <summary>
/// Cria pontos de segurança FICTÍCIOS para demonstração (esses locais ainda não existem de verdade).
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.SafeZones.AnyAsync(cancellationToken))
            return;

        var zones = new[]
        {
            new SafeZone("Centro de Acolhimento Sé (fictício)", "Praça da Sé, Centro, São Paulo",
                -23.5503, -46.6339, 500,
                SafeZoneServices.AirConditioning | SafeZoneServices.Heating | SafeZoneServices.LowCostWater | SafeZoneServices.FirstAid | SafeZoneServices.GovernmentManaged),
            new SafeZone("Abrigo Pinheiros (fictício)", "Pinheiros, São Paulo",
                -23.5670, -46.7020, 300,
                SafeZoneServices.AirConditioning | SafeZoneServices.LowCostWater),
            new SafeZone("Abrigo Elevado Penha (fictício)", "Penha, São Paulo",
                -23.5290, -46.5400, 400,
                SafeZoneServices.FloodSafeShelter | SafeZoneServices.FirstAid | SafeZoneServices.GovernmentManaged),
            new SafeZone("Centro Climático Santo Amaro (fictício)", "Santo Amaro, São Paulo",
                -23.6540, -46.7090, 350,
                SafeZoneServices.AirConditioning | SafeZoneServices.Heating | SafeZoneServices.FloodSafeShelter),
            new SafeZone("Ponto Itaquera (fictício)", "Itaquera, São Paulo",
                -23.5410, -46.4710, 250,
                SafeZoneServices.FloodSafeShelter | SafeZoneServices.LowCostWater),
            new SafeZone("Ginásio de Apoio Santana (fictício)", "Santana, São Paulo",
                -23.5017, -46.6290, 600,
                SafeZoneServices.Heating | SafeZoneServices.FloodSafeShelter | SafeZoneServices.FirstAid | SafeZoneServices.GovernmentManaged)
        };

        await db.SafeZones.AddRangeAsync(zones, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}
