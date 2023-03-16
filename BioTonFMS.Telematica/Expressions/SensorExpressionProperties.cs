using BioTonFMS.Domain;

namespace BioTonFMS.Telematica.Expressions;

public readonly struct SensorExpressionProperties : ISensorExpressionProperties
{
    public readonly Sensor Sensor;

    public string Name => Sensor.Name;
    public string Formula => Sensor.Formula;
    public bool UseFallbacks => Sensor.UseLastReceived;
    public ValidationTypeEnum? ValidationType => Sensor.ValidationType;
    public string? ValidatorName { get; }

    public SensorExpressionProperties(Sensor sensor, string? validatorName = null) : this()
    {
        Sensor = sensor;
        ValidatorName = validatorName;
    }
}