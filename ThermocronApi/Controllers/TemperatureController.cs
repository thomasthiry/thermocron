using Microsoft.AspNetCore.Mvc;
using ThermocronApi.DTOs;
using ThermocronApi.Services;

namespace ThermocronApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TemperatureController : ControllerBase
{
    private readonly TemperatureService _temperatureService;

    public TemperatureController(TemperatureService temperatureService)
    {
        _temperatureService = temperatureService;
    }

    [HttpGet("measures")]
    public async Task<ActionResult<List<MeasureDto>>> GetMeasures(
        [FromQuery] int? deviceId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string interval = "hour")
    {
        var fromDate = from ?? DateTime.Now.AddDays(-1);
        var toDate = to ?? DateTime.Now;

        if (fromDate >= toDate)
        {
            return BadRequest("La date de début doit être antérieure à la date de fin.");
        }

        var measures = await _temperatureService.GetMeasuresAsync(deviceId, fromDate, toDate, interval);
        return Ok(measures);
    }

    [HttpGet("latest")]
    public async Task<ActionResult<MeasureDto>> GetLatest([FromQuery] int? deviceId)
    {
        var latest = await _temperatureService.GetLatestMeasureAsync(deviceId);
        
        if (latest == null)
        {
            return NotFound("Aucune mesure trouvée.");
        }

        return Ok(latest);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<TemperatureStatsDto>> GetStats(
        [FromQuery] int? deviceId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var fromDate = from ?? DateTime.Now.AddDays(-1);
        var toDate = to ?? DateTime.Now;

        if (fromDate >= toDate)
        {
            return BadRequest("La date de début doit être antérieure à la date de fin.");
        }

        var stats = await _temperatureService.GetStatsAsync(deviceId, fromDate, toDate);
        return Ok(stats);
    }
}
