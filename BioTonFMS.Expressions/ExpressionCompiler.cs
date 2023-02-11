using System.Globalization;
using System.Linq.Expressions;
using BioTonFMS.Expressions.AST;

namespace BioTonFMS.Expressions;

public static class ExpressionCompiler
{
    public static Expression Compile(AstNode node, IDictionary<string, Type> parameters)
    {
        var expressionParameters = GetParameters(node, parameters);
        var expression = TranslateRec(node, expressionParameters);
        var lambda = Expression.Lambda(expression, expressionParameters.Values);
        return lambda;
    }

    public static object? Execute(LambdaExpression lambda, IDictionary<string, object> arguments)
    {
        var args = lambda.Parameters.Select(p => arguments[p.Name!]);
        return lambda?.Compile().DynamicInvoke(args.ToArray());
    }
    
    private static IDictionary<string, ParameterExpression> GetParameters(AstNode node, IDictionary<string, Type> parameters)
    {
        var variables = GetVariables(node);
        var result = new Dictionary<string, ParameterExpression>(8);
        foreach (var name in variables)
        {
            if (parameters.TryGetValue(name, out var type))
            {
                result.Add(name, Expression.Parameter(parameters[name], name));
            }
            else
            {
                throw new FormatException($"Variable with name {name} doesn't exist!");
            }
        }
        return result;
    }
    private static IEnumerable<string> GetVariables(AstNode node)
    {
        var result = new HashSet<string>();
        GetVariablesRec(node, result);
        return result;
    }
    private static void GetVariablesRec(AstNode? node, ISet<string> result)
    {
        var stack = new Stack<AstNode>(8);
        while (true)
        {
            switch (node)
            {
                case BinaryOperation v:
                    node = v.LeftNode;
                    stack.Push(v.RightNode);
                    continue;
                case UnaryOperation v:
                    node = v.Node;
                    continue;
                case Variable v:
                    result.Add(v.Name);
                    break;
            }
            if (stack.Count == 0)
                break;
            node = stack.Pop();
        }
    }

    private static Expression TranslateRec(AstNode node, IDictionary<string, ParameterExpression> parameters)
    {
        return node switch
        {
            BinaryOperation v => v.Operation switch
            {
                BinaryOperationEnum.Addition => Expression.Add(TranslateRec(v.LeftNode, parameters), TranslateRec(v.RightNode, parameters)),
                BinaryOperationEnum.Subtraction => Expression.Subtract(TranslateRec(v.LeftNode, parameters),
                    TranslateRec(v.RightNode, parameters)),
                BinaryOperationEnum.Multiplication =>
                    Expression.Multiply(TranslateRec(v.LeftNode, parameters), TranslateRec(v.RightNode, parameters)),
                BinaryOperationEnum.Division => Expression.Divide(TranslateRec(v.LeftNode, parameters),
                    TranslateRec(v.RightNode, parameters)),
                _ => throw new ArgumentException("Invalid AST: Invalid type of binary operation!", nameof(node))
            },
            UnaryOperation v => v.Operation switch
            {
                UnaryOperationEnum.Negation => Expression.Negate(TranslateRec(v.Node, parameters)),
                UnaryOperationEnum.Parentheses => TranslateRec(v.Node, parameters),
                _ => throw new ArgumentException("Invalid AST: Invalid type of unary operation!", nameof(v.Operation))
            },
            Literal v => v.Type switch
            {
                LiteralEnum.Decimal => Expression.Constant(double.Parse(v.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo),
                    typeof( double )),
                _ => throw new ArgumentException("Invalid AST: Invalid type of literal!", nameof(v.Type))
            },
            Variable v => parameters[v.Name],
            _ => throw new ArgumentException("Invalid AST: Unknown type of node!")
        };
    }
}
