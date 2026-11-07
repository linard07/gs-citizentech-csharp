using CitizenTech.Api.Application.Dtos;
using CitizenTech.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CitizenTech.Api.Controllers;

/// <summary>Pontos de segurança (no protótipo, todos fictícios).</summary>
[ApiController]
[Route("api/safe-zones")]
[Produces("application/json")]
public class SafeZonesController : ControllerBase
{
    private readonly ISafeZoneService _service;

    public SafeZonesController(ISafeZoneService service) => _service = service;

    /// <summary>Lista os pontos de segurança (por padrão, só os ativos).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SafeZoneResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SafeZoneResponse>>> List(
        [FromQuery] bool onlyActive = true, CancellationToken cancellationToken = default)
        => Ok(await _service.ListAsync(onlyActive, cancellationToken));

    /// <summary>Busca um ponto de segurança pelo identificador.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SafeZoneResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SafeZoneResponse>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _service.GetAsync(id, cancellationToken));

    /// <summary>Cadastra um ponto de segurança.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(SafeZoneResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SafeZoneResponse>> Create([FromBody] SafeZoneRequest request, CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Atualiza os dados de um ponto de segurança.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(SafeZoneResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<SafeZoneResponse>> Update(int id, [FromBody] SafeZoneRequest request, CancellationToken cancellationToken)
        => Ok(await _service.UpdateAsync(id, request, cancellationToken));

    /// <summary>Desativa o ponto (exclusão lógica, preservando o histórico de entradas).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        await _service.DeactivateAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Registra a saída de pessoas do ponto, liberando vagas.</summary>
    [HttpPost("{id:int}/check-out")]
    [ProducesResponseType(typeof(SafeZoneResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<SafeZoneResponse>> CheckOut(int id, [FromBody] CheckOutRequest request, CancellationToken cancellationToken)
        => Ok(await _service.CheckOutAsync(id, request, cancellationToken));
}
