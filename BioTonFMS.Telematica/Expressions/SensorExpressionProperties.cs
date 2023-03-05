using BioTonFMS.Domain;
using BioTonFMS.Expressions;

namespace BioTonFMS.Telematica.Expressions;

public readonly struct SensorExpressionProperties : ISensorExpressionProperties
{
    public readonly Sensor Sensor;
    
    public string Name => Sensor.Name;
    public string Formula => Sensor.Formula;
    public bool UseFallbacks => Sensor.UseLastReceived;

    public SensorExpressionProperties(Sensor sensor, string? validatorName = null) : this()
    {
        Sensor = sensor;
    }
}
public interface ISensorExpressionProperties : IExpressionProperties
{
    /// <summary>
    /// If it is true then in cases when arguments passed to the executed expression represent fallback values
    /// (values taken from previous messages) expression will use those values, and if this option is false
    /// it will replace those values with nulls 
    /// </summary>
    bool UseFallbacks { get; }
}
