using System.Linq.Expressions;

namespace BioTonFMS.MessageProcessing;

public static class SensorExpressionBuilderExtensions
{
    /// <summary>
    /// Builds expression tree which implements function Latest which unconditionally extracts value
    /// from TagData. Now it is used only when UseFallbacks option is true, but in the future it could
    /// be used in formulas explicitly (currently our parser can not parse functions but perhaps
    /// it should in the future) 
    /// </summary>
    /// <param name="tagValue">Expression tree representing value of parameter</param>
    /// <returns>Expression tree which extracts value from parameter</returns>
    public static Expression Latest(this ParameterExpression tagValue)
    {
        return Expression.Property(tagValue, "Value");
    }

    /// <summary>
    /// Builds expression tree which implements function Current which extracts value from TagData
    /// only in those cases when the value is not a fallback value. Now it is used only when
    /// UseFallbacks option is false, but in the future it could be used in formulas explicitly
    /// (currently our parser can not parse functions but perhaps it should in the future) 
    /// </summary>
    /// <param name="tagValue">Expression tree representing value of parameter</param>
    /// <returns>Expression tree which extracts value from parameter</returns>
    public static Expression Current(this ParameterExpression tagValue)
    {
        return Expression.Condition(
                    Expression.Property(tagValue, "IsFallback"),
                    Expression.Constant(null, typeof( double? )),
                    Expression.Property(tagValue, "Value"),
                    typeof( double? ));
    }
}
