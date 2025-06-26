using Microsoft.AspNetCore.Mvc;
using ThermocronApi.DTOs;
using ThermocronApi.Services;

namespace ThermocronApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeviceController : ControllerBase
{
    private readonly TemperatureService _temperatureService;

    public DeviceController(TemperatureService temperatureService)
    {
        _temperatureService = temperatureService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DeviceDto>>> GetDevices()
    {
        var devices = await _temperatureService.GetDevicesAsync();
        return Ok(devices);
    }
}
