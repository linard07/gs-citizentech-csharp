namespace CitizenTech.Api.Application;

/// <summary>Recurso solicitado não existe.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
