// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace BioTonFMS.Expressions;

public struct TagData<TValue>
{
    public TagData(double? value, bool isFallback = false)
    {
        Value = value;
        IsFallback = isFallback;
    }

    public double? Value { get; }
    public bool IsFallback { get; }
}
