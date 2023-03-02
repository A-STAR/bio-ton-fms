// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace BioTonFMS.Expressions;

/// <summary>
/// Represents arguments which are passed to expressions. Apart from main value also contains flag which
/// describes whether the value is fallback value or not. Fallback values are values which are taken
/// from previous message (is transitive) because current message didn't contain update for this value
/// for some reason. 
/// </summary>
/// <typeparam name="TValue">Is not used in current implementation</typeparam>
public struct TagData<TValue>
{
    public TagData(double? value, bool isFallback = false)
    {
        Value = value;
        IsFallback = isFallback;
    }
    
    /// <summary>
    /// Value of parameter
    /// </summary>
    public double? Value { get; }
    
    /// <summary>
    /// Tells us, whether the value is fallback value or not :) 
    /// </summary>
    public bool IsFallback { get; }
}
