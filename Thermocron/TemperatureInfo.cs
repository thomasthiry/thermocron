public class TemperatureInfo
{
    public double MeasuredTemperature { get; }
    public double TargetTemperature { get; }
    public double OutdoorTemperature { get; }

    public TemperatureInfo(double measured, double target, double outdoor)
    {
        MeasuredTemperature = measured;
        TargetTemperature = target;
        OutdoorTemperature = outdoor;
    }

    public override string ToString()
    {
        return $"Measured: {MeasuredTemperature}°C, Target: {TargetTemperature}°C, Outdoor: {OutdoorTemperature}°C";
    }
}