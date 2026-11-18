
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oasis.Api.Services;
using Oasis.Domain.Models;

namespace Oasis.Api.Controllers;

[ApiController]
[Route("api/heat-alerts")]
public class HeatAlertsController : ControllerBase
{
    private readonly HeatAlertService _service;

    public HeatAlertsController(HeatAlertService service)
    {
        _service = service;
    }

    // GET: api/heat-alerts/analyze?temperatureC=35
    [HttpGet("analyze")]
    [ProducesResponseType(
        typeof(HeatAlert),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<HeatAlert> Analyze(
        [FromQuery] double? temperatureC)
    {
        if (!temperatureC.HasValue ||
            double.IsNaN(temperatureC.Value) ||
            double.IsInfinity(temperatureC.Value) ||
            temperatureC.Value < -90 ||
            temperatureC.Value > 60)
        {
            return BadRequest(new
            {
                message = "Informe uma temperatura válida entre -90 e 60°C."
            });
        }

        var result = _service.Analyze(temperatureC.Value);

        return Ok(result);
    }
}
