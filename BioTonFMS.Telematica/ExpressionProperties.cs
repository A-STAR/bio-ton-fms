using BioTonFMS.Domain;
using BioTonFMS.Expressions;

namespace BioTonFMS.Telematica;

public readonly struct ExpressionProperties : IExpressionProperties
{
    public readonly Sensor Sensor;

    public string Name => Sensor.Name;
    public string Formula => Sensor.Formula;
    public bool UseFallbacks => Sensor.UseLastReceived;

    public ExpressionProperties(Sensor sensor) : this()
    {
        Sensor = sensor;
    }
}
