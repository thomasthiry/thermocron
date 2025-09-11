namespace ThermocronApi.Extensions;

public static class TemperatureExtensions
{
    public static double RoundTemperature(this double temperature)
    {
        return Math.Round(temperature, 1);
    }
}