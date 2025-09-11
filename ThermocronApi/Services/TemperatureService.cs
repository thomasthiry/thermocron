using Microsoft.EntityFrameworkCore;
using ThermocronApi.Data;
using ThermocronApi.DTOs;
using ThermocronApi.Extensions;

namespace ThermocronApi.Services;

public class TemperatureService
{
    private readonly AppDbContext _context;

    public TemperatureService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DeviceDto>> GetDevicesAsync()
    {
        return await _context.Devices
            .Select(d => new DeviceDto
            {
                Id = d.Id,
                Name = d.Name,
                LastMeasureTime = d.Measures.OrderByDescending(m => m.Timestamp).FirstOrDefault().Timestamp,
                LastMeasuredTemperature = d.Measures.OrderByDescending(m => m.Timestamp).FirstOrDefault().MeasuredTemperature.RoundTemperature()
            })
            .ToListAsync();
    }

    public async Task<List<MeasureDto>> GetMeasuresAsync(int? deviceId, DateTime from, DateTime to, string interval = "hour")
    {
        var query = _context.Measures.AsQueryable();

        if (deviceId.HasValue)
        {
            query = query.Where(m => m.DeviceId == deviceId.Value);
        }

        query = query.Where(m => m.Timestamp >= from && m.Timestamp <= to);

        // Agrégation selon l'intervalle
        if (interval == "day")
        {
            return await query
                .GroupBy(m => new { m.DeviceId, Date = m.Timestamp.Date })
                .Select(g => new MeasureDto
                {
                    Timestamp = g.Key.Date,
                    MeasuredTemperature = g.Average(m => m.MeasuredTemperature).RoundTemperature(),
                    TargetTemperature = g.Average(m => m.TargetTemperature).RoundTemperature(),
                    OutdoorTemperature = g.Average(m => m.OutdoorTemperature).RoundTemperature()
                })
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }
        else if (interval == "hour")
        {
            return await query
                .GroupBy(m => new { 
                    m.DeviceId, 
                    Year = m.Timestamp.Year,
                    Month = m.Timestamp.Month,
                    Day = m.Timestamp.Day,
                    Hour = m.Timestamp.Hour
                })
                .Select(g => new MeasureDto
                {
                    Timestamp = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day, g.Key.Hour, 0, 0),
                    MeasuredTemperature = g.Average(m => m.MeasuredTemperature).RoundTemperature(),
                    TargetTemperature = g.Average(m => m.TargetTemperature).RoundTemperature(),
                    OutdoorTemperature = g.Average(m => m.OutdoorTemperature).RoundTemperature()
                })
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }
        else
        {
            // Données brutes
            return await query
                .Select(m => new MeasureDto
                {
                    Timestamp = m.Timestamp,
                    MeasuredTemperature = m.MeasuredTemperature,
                    TargetTemperature = m.TargetTemperature,
                    OutdoorTemperature = m.OutdoorTemperature
                })
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }
    }

    public async Task<TemperatureStatsDto> GetStatsAsync(int? deviceId, DateTime from, DateTime to)
    {
        var query = _context.Measures.AsQueryable();

        if (deviceId.HasValue)
        {
            query = query.Where(m => m.DeviceId == deviceId.Value);
        }

        query = query.Where(m => m.Timestamp >= from && m.Timestamp <= to);

        if (!await query.AnyAsync())
        {
            return new TemperatureStatsDto();
        }

        return await query
            .GroupBy(m => 1)
            .Select(g => new TemperatureStatsDto
            {
                MinMeasured = g.Min(m => m.MeasuredTemperature).RoundTemperature(),
                MaxMeasured = g.Max(m => m.MeasuredTemperature).RoundTemperature(),
                AvgMeasured = g.Average(m => m.MeasuredTemperature).RoundTemperature(),
                MinTarget = g.Min(m => m.TargetTemperature).RoundTemperature(),
                MaxTarget = g.Max(m => m.TargetTemperature).RoundTemperature(),
                AvgTarget = g.Average(m => m.TargetTemperature).RoundTemperature(),
                MinOutdoor = g.Min(m => m.OutdoorTemperature).RoundTemperature(),
                MaxOutdoor = g.Max(m => m.OutdoorTemperature).RoundTemperature(),
                AvgOutdoor = g.Average(m => m.OutdoorTemperature).RoundTemperature(),
                TotalMeasures = g.Count()
            })
            .FirstAsync();
    }

    public async Task<MeasureDto?> GetLatestMeasureAsync(int? deviceId)
    {
        var query = _context.Measures.AsQueryable();

        if (deviceId.HasValue)
        {
            query = query.Where(m => m.DeviceId == deviceId.Value);
        }

        var latest = await query
            .OrderByDescending(m => m.Timestamp)
            .FirstOrDefaultAsync();

        if (latest == null)
            return null;

        return new MeasureDto
        {
            Timestamp = latest.Timestamp,
            MeasuredTemperature = latest.MeasuredTemperature.RoundTemperature(),
            TargetTemperature = latest.TargetTemperature.RoundTemperature(),
            OutdoorTemperature = latest.OutdoorTemperature.RoundTemperature()
        };
    }
}
