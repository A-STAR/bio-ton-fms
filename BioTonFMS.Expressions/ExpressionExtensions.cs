using System.Linq.Expressions;

namespace BioTonFMS.Expressions;

public static class ExpressionExtensions
{
    public static Expression Adjust(this BinaryExpression operation)
    {
        return Expression.Condition(
            Expression.And(
                Expression.Property(operation.Left, "HasValue"),
                Expression.Property(operation.Right, "HasValue")),
            Expression.ConvertChecked(operation, typeof( double? )),
            Expression.Constant(null, typeof( double? )),
            typeof( double? ));
    }
    public static Expression Adjust(this UnaryExpression operation)
    {
        return Expression.Condition(
            Expression.Property(operation.Operand, "HasValue"),
            Expression.ConvertChecked(operation, typeof( double? )),
            Expression.Constant(null, typeof( double? )),
            typeof( double? ));
    }
    public static Expression Latest(ParameterExpression tagValue)
    {
        return Nullable.GetUnderlyingType(tagValue.Type) == null
            ? Expression.Property(tagValue, "Value")
            : tagValue;
    }
    public static Expression Current(ParameterExpression tagValue)
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
    public static Expression WrapParameter(ParameterExpression parameter, bool useFallbacks)
    {
        return useFallbacks ? Latest(parameter) : Current(parameter);
    }
}
