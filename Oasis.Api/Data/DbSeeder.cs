
using Microsoft.EntityFrameworkCore;
using Oasis.Domain.Models;

namespace Oasis.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(OasisDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.ProtectionPoints.AnyAsync())
            return;

        var points = new List<ProtectionPoint>
        {
            Create("Oasis Demo 01 - Água", ProtectionType.FreeWater, -23.5432, -46.6427),
            Create("Oasis Demo 02 - Sombra", ProtectionType.Shade, -23.5434, -46.6425),
            Create("Oasis Demo 03 - Comércio", ProtectionType.Commerce, -23.5430, -46.6429),
            Create("Oasis Demo 04 - Água", ProtectionType.FreeWater, -23.5428, -46.6424),
            Create("Oasis Demo 05 - Sombra", ProtectionType.Shade, -23.5436, -46.6431),
            Create("Oasis Demo 06 - Comércio", ProtectionType.Commerce, -23.5429, -46.6433),
            Create("Oasis Demo 07 - Água", ProtectionType.FreeWater, -23.5437, -46.6422),
            Create("Oasis Demo 08 - Sombra", ProtectionType.Shade, -23.5426, -46.6428),
            Create("Oasis Demo 09 - Comércio", ProtectionType.Commerce, -23.5435, -46.6435),
            Create("Oasis Demo 10 - Água", ProtectionType.FreeWater, -23.5427, -46.6430)
        };

        await context.ProtectionPoints.AddRangeAsync(points);
        await context.SaveChangesAsync();
    }

    private static ProtectionPoint Create(
        string name,
        ProtectionType type,
        double latitude,
        double longitude)
    {
        return new ProtectionPoint
        {
            Name = name,
            Description = "DADO FICTÍCIO PARA DEMONSTRAÇÃO. "
                + "Não representa um serviço real ou verificado.",
            Address = "Região da Praça da República, São Paulo - SP "
                + "(localização simulada)",
            Latitude = latitude,
            Longitude = longitude,
            Type = type,
            IsActive = true
        };
    }
}
