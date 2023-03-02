using BioTonFMS.Domain;
using BioTonFMS.Expressions;

namespace BioTonFMS.Telematica;

public readonly struct ExpressionProperties : IExpressionProperties
{
    public readonly Sensor Sensor;

    public string Name => Sensor.Name;
    public string Formula => Sensor.Formula;
    public bool UseFallbacks => Sensor.UseLastReceived;
    public bool HasValidator => Sensor.ValidatorId.HasValue;
    public string? ValidatorName { get; }

    public ExpressionProperties(Sensor sensor, string? validatorName = null) : this()
    {
        Sensor = sensor;
        ValidatorName = validatorName;
    }
}
