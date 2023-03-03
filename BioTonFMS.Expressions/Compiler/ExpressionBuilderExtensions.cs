using System.Linq.Expressions;

namespace BioTonFMS.Expressions.Compilation;

public static class ExpressionBuilderExtensions
{
    /// <summary>
    /// Adjusts binary operations in a way which allows us to process null operands gracefully.
    /// When binary expression receives at least one null operand it will return null as a result,
    /// but won't throw an exception.
    /// </summary>
    /// <param name="operation">Binary operation which should be adjusted</param>
    /// <returns>Expression tree of the adjusted binary operation</returns>
    public static Expression WrapWithNullHandler(this BinaryExpression operation)
    {
        return Expression.Condition(
            Expression.And(
                Expression.Property(operation.Left, "HasValue"),
                Expression.Property(operation.Right, "HasValue")),
            Expression.ConvertChecked(operation, typeof( double? )),
            Expression.Constant(null, typeof( double? )),
            typeof( double? ));
    }

    /// <summary>
    /// Adjusts unary operations in a way which allows us to process null operands gracefully.
    /// When unary expression receives null operand it will return null as a result, but won't
    /// throw an exception.
    /// </summary>
    /// <param name="operation">Unary operation which should be adjusted</param>
    /// <returns>Expression tree of the adjusted unary operation</returns>
    public static Expression WrapWithNullHandler(this UnaryExpression operation)
    {
        return Expression.Condition(
            Expression.Property(operation.Operand, "HasValue"),
            Expression.ConvertChecked(operation, typeof( double? )),
            Expression.Constant(null, typeof( double? )),
            typeof( double? ));
    }

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
        return Nullable.GetUnderlyingType(tagValue.Type) == null
            ? Expression.Property(tagValue, "Value")
            : tagValue;
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
        return Nullable.GetUnderlyingType(tagValue.Type) != null
            ? tagValue
            : tagValue.Type == typeof( TagData<double> )
                ? Expression.Condition(
                    Expression.Property(tagValue, "IsFallback"),
                    Expression.Constant(null, typeof( double? )),
                    Expression.Property(tagValue, "Value"),
                    typeof( double? ))
                : throw new Exception("Internal error!");
    }
}
