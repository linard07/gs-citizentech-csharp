using CitizenTech.Api.Application.Dtos;
using CitizenTech.Api.Application.Services;
using CitizenTech.Api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CitizenTech.Api.Controllers;

/// <summary>Alertas de eventos extremos (chuva forte, alagamento, calor e frio extremos).</summary>
[ApiController]
[Route("api/alerts")]
[Produces("application/json")]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _service;

    public AlertsController(IAlertService service) => _service = service;

    /// <summary>Lista alertas com filtros opcionais e paginação.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AlertResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AlertResponse>>> Search(
        [FromQuery] AlertType? type,
        [FromQuery] AlertStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
        => Ok(await _service.SearchAsync(type, status, page, pageSize, cancellationToken));

    /// <summary>Busca um alerta pelo identificador.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AlertResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertResponse>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _service.GetAsync(id, cancellationToken));

    /// <summary>
    /// Avalia uma medição. Se ela ultrapassar o limiar do tipo de evento, cria o alerta (201);
    /// caso contrário, responde 200 informando que nenhum alerta foi criado.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AlertEvaluationResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(AlertEvaluationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AlertEvaluationResult>> Create(
        [FromBody] CreateAlertRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.EvaluateAndCreateAsync(request, cancellationToken);

        if (!result.AlertCreated)
            return Ok(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Alert!.Id }, result);
    }

    /// <summary>Encerra um alerta ativo.</summary>
    [HttpPost("{id:int}/close")]
    [ProducesResponseType(typeof(AlertResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<AlertResponse>> Close(int id, CancellationToken cancellationToken)
        => Ok(await _service.CloseAsync(id, cancellationToken));

    /// <summary>
    /// Pontos de segurança mais próximos, compatíveis com o tipo do alerta e com vagas.
    /// Se latitude/longitude não forem informadas, usa a localização do próprio alerta.
    /// </summary>
    [HttpGet("{id:int}/safe-zones")]
    [ProducesResponseType(typeof(IReadOnlyList<NearbySafeZoneResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<NearbySafeZoneResponse>>> GetNearestSafeZones(
        int id,
        [FromQuery] double? latitude,
        [FromQuery] double? longitude,
        [FromQuery] int limit = 3,
        CancellationToken cancellationToken = default)
        => Ok(await _service.GetNearestSafeZonesAsync(id, latitude, longitude, limit, cancellationToken));

    /// <summary>Registra a entrada de pessoas em um ponto de segurança durante o alerta.</summary>
    [HttpPost("{id:int}/check-ins")]
    [ProducesResponseType(typeof(CheckInResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CheckInResponse>> RegisterCheckIn(
        int id, [FromBody] CheckInRequest request, CancellationToken cancellationToken)
    {
        var response = await _service.RegisterCheckInAsync(id, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>Estatísticas gerais: alertas por tipo e pessoas abrigadas.</summary>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(AlertStatisticsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AlertStatisticsResponse>> GetStatistics(CancellationToken cancellationToken)
        => Ok(await _service.GetStatisticsAsync(cancellationToken));
}
