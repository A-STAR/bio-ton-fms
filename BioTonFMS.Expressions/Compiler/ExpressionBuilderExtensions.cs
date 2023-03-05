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
}
