namespace CitizenTech.Api.Domain;

/// <summary>
/// Ponto de segurança para onde a população é direcionada durante um evento extremo
/// (área refrigerada, abrigo contra alagamento, água a preço reduzido etc.).
/// No protótipo os pontos são fictícios (<see cref="IsFictitious"/>).
/// </summary>
public class SafeZone : Entity
{
    private SafeZone() // usado pelo EF Core
    {
        Name = string.Empty;
        Address = string.Empty;
    }

    public SafeZone(string name, string address, double latitude, double longitude,
        int capacity, SafeZoneServices services, bool isFictitious = true)
    {
        Name = string.Empty;
        Address = string.Empty;
        ApplyDetails(name, address, latitude, longitude, capacity, services);
        IsFictitious = isFictitious;
        IsActive = true;
    }

    public string Name { get; private set; }
    public string Address { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public int Capacity { get; private set; }
    public int CurrentOccupancy { get; private set; }
    public SafeZoneServices Services { get; private set; }
    public bool IsFictitious { get; private set; }
    public bool IsActive { get; private set; }

    public int AvailableSpots => Capacity - CurrentOccupancy;

    public bool Supports(AlertType type) => (Services & type.RequiredService()) != 0;

    public void Update(string name, string address, double latitude, double longitude,
        int capacity, SafeZoneServices services)
        => ApplyDetails(name, address, latitude, longitude, capacity, services);

    public void Deactivate() => IsActive = false;

    public void CheckIn(int people)
    {
        if (!IsActive)
            throw new DomainException($"O ponto \"{Name}\" está desativado.");
        if (people < 1)
            throw new DomainException("A quantidade de pessoas deve ser maior que zero.");
        if (people > AvailableSpots)
            throw new DomainException($"O ponto \"{Name}\" tem apenas {AvailableSpots} vagas disponíveis.");

        CurrentOccupancy += people;
    }

    public void CheckOut(int people)
    {
        if (people < 1)
            throw new DomainException("A quantidade de pessoas deve ser maior que zero.");
        if (people > CurrentOccupancy)
            throw new DomainException($"O ponto \"{Name}\" tem apenas {CurrentOccupancy} pessoas registradas.");

        CurrentOccupancy -= people;
    }

    private void ApplyDetails(string name, string address, double latitude, double longitude,
        int capacity, SafeZoneServices services)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome do ponto de segurança é obrigatório.");
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("O endereço do ponto de segurança é obrigatório.");
        if (latitude is < -90 or > 90 || longitude is < -180 or > 180)
            throw new DomainException("Coordenadas inválidas.");
        if (capacity < 1)
            throw new DomainException("A capacidade deve ser de pelo menos 1 pessoa.");
        if (capacity < CurrentOccupancy)
            throw new DomainException($"A capacidade não pode ser menor que a ocupação atual ({CurrentOccupancy}).");

        Name = name.Trim();
        Address = address.Trim();
        Latitude = latitude;
        Longitude = longitude;
        Capacity = capacity;
        Services = services;
    }
}
