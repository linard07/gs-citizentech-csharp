
using System.ComponentModel.DataAnnotations;

namespace Oasis.Domain.Models;

public class ProtectionPoint
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, ErrorMessage = "O nome deve ter até 150 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição deve ter até 500 caracteres.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "O endereço é obrigatório.")]
    [StringLength(250, ErrorMessage = "O endereço deve ter até 250 caracteres.")]
    public string Address { get; set; } = string.Empty;

    [Range(-90, 90, ErrorMessage = "Latitude inválida.")]
    public double Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "Longitude inválida.")]
    public double Longitude { get; set; }

    [EnumDataType(typeof(ProtectionType),
        ErrorMessage = "Tipo de proteção inválido.")]
    public ProtectionType Type { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum ProtectionType
{
    Shade = 1,
    FreeWater = 2,
    Commerce = 3
}
