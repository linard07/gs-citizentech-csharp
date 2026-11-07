namespace CitizenTech.Api.Domain;

/// <summary>Classe base abstrata de todas as entidades persistidas.</summary>
public abstract class Entity
{
    public int Id { get; protected set; }
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
}
