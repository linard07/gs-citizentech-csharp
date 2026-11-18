
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oasis.Api.Services;
using Oasis.Domain.Interfaces;
using Oasis.Domain.Models;

namespace Oasis.Api.Controllers;

[ApiController]
[Route("api/protection-points")]
public class ProtectionPointsController : ControllerBase
{
    private readonly IProtectionPointRepository _repository;

    public ProtectionPointsController(
        IProtectionPointRepository repository)
    {
        _repository = repository;
    }

    // GET: api/protection-points
    // Lista todos os pontos de proteção
    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<ProtectionPoint>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProtectionPoint>>> GetAll()
    {
        var points = await _repository.GetAllAsync();

        return Ok(points);
    }

    // GET: api/protection-points/nearby
    // Busca pontos ativos próximos à localização informada
    [HttpGet("nearby")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 1)
    {
        if (double.IsNaN(latitude) ||
            double.IsInfinity(latitude) ||
            double.IsNaN(longitude) ||
            double.IsInfinity(longitude) ||
            latitude < -90 || latitude > 90 ||
            longitude < -180 || longitude > 180)
        {
            return BadRequest(new
            {
                message = "Coordenadas inválidas."
            });
        }

        if (double.IsNaN(radiusKm) ||
            double.IsInfinity(radiusKm) ||
            radiusKm <= 0 || radiusKm > 50)
        {
            return BadRequest(new
            {
                message = "O raio deve ser maior que 0 e até 50 km."
            });
        }

        var points = await _repository.GetAllAsync();

        var nearby = points
            .Where(p => p.IsActive)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Address,
                p.Latitude,
                p.Longitude,
                p.Type,
                DistanceKm = GeoDistanceService.CalculateKm(
                    latitude,
                    longitude,
                    p.Latitude,
                    p.Longitude)
            })
            .Where(p => p.DistanceKm <= radiusKm)
            .OrderBy(p => p.DistanceKm)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Address,
                p.Latitude,
                p.Longitude,
                p.Type,
                DistanceKm = Math.Round(p.DistanceKm, 3)
            })
            .ToList();

        return Ok(nearby);
    }

    // GET: api/protection-points/{id}
    // Busca um ponto de proteção pelo ID
    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(ProtectionPoint),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProtectionPoint>> GetById(Guid id)
    {
        var point = await _repository.GetByIdAsync(id);

        if (point is null)
        {
            return NotFound(new
            {
                message = "Ponto de proteção não encontrado."
            });
        }

        return Ok(point);
    }

    // POST: api/protection-points
    // Cadastra um novo ponto de proteção
    [HttpPost]
    [ProducesResponseType(
        typeof(ProtectionPoint),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProtectionPoint>> Create(
        [FromBody] ProtectionPoint point)
    {
        if (string.IsNullOrWhiteSpace(point.Name) ||
            string.IsNullOrWhiteSpace(point.Address))
        {
            return BadRequest(new
            {
                message = "Nome e endereço são obrigatórios."
            });
        }

        if (point.Latitude < -90 || point.Latitude > 90 ||
            point.Longitude < -180 || point.Longitude > 180)
        {
            return BadRequest(new
            {
                message = "Coordenadas inválidas."
            });
        }

        if (!Enum.IsDefined(point.Type))
        {
            return BadRequest(new
            {
                message = "Tipo de proteção inválido."
            });
        }

        point.Id = Guid.NewGuid();
        point.CreatedAt = DateTime.UtcNow;

        var created = await _repository.AddAsync(point);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created
        );
    }

    // PUT: api/protection-points/{id}
    // Atualiza um ponto de proteção existente
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] ProtectionPoint point)
    {
        if (id != point.Id)
        {
            return BadRequest(new
            {
                message = "O ID informado não corresponde ao ponto."
            });
        }

        if (string.IsNullOrWhiteSpace(point.Name) ||
            string.IsNullOrWhiteSpace(point.Address))
        {
            return BadRequest(new
            {
                message = "Nome e endereço são obrigatórios."
            });
        }

        if (point.Latitude < -90 || point.Latitude > 90 ||
            point.Longitude < -180 || point.Longitude > 180)
        {
            return BadRequest(new
            {
                message = "Coordenadas inválidas."
            });
        }

        if (!Enum.IsDefined(point.Type))
        {
            return BadRequest(new
            {
                message = "Tipo de proteção inválido."
            });
        }

        var updated = await _repository.UpdateAsync(point);

        if (!updated)
        {
            return NotFound(new
            {
                message = "Ponto de proteção não encontrado."
            });
        }

        return NoContent();
    }

    // DELETE: api/protection-points/{id}
    // Remove um ponto de proteção
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _repository.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Ponto de proteção não encontrado."
            });
        }

        return NoContent();
    }
}
