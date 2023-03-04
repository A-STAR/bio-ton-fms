﻿using System.Globalization;
using System.Linq.Expressions;
using BioTonFMS.Expressions.Ast;

namespace BioTonFMS.Expressions.Compilation;

public class ExpressionBuilder : IExpressionBuilder
{
    private readonly IExpressionProperties? _expressionProperties;

    public ExpressionBuilder(IExpressionProperties? expressionProperties = null)
    {
        _expressionProperties = expressionProperties;
    }

    public Expression BuildBinary(BinaryOperationEnum operation, Expression leftOperand, Expression rightOperand)
    {
        var expression = operation switch
        {
            BinaryOperationEnum.Addition => Expression.Add(leftOperand, rightOperand),
            BinaryOperationEnum.Subtraction => Expression.Subtract(leftOperand, rightOperand),
            BinaryOperationEnum.Multiplication => Expression.Multiply(leftOperand, rightOperand),
            BinaryOperationEnum.Division => Expression.Divide(leftOperand, rightOperand),
            _ => throw new ArgumentException("Invalid AST: Invalid type of binary operation!", nameof(operation))
        };
        return expression.WrapWithNullHandler();
    }

    public Expression BuildUnary(UnaryOperationEnum operation, Expression operand)
    {
        var expression = operation switch
        {
            UnaryOperationEnum.Negation => Expression.Negate(operand).WrapWithNullHandler(),
            UnaryOperationEnum.Parentheses => operand,
            _ => throw new Exception("Invalid AST: Invalid type of unary operation!")
        };
        return expression;
    }

    public Expression BuildFunction(string name, IEnumerable<Expression> arguments)
    {
        throw new Exception($"There is no function with name {name}!");
    }

    public Expression BuildConstant(LiteralEnum type, string literalString)
    {
        var expression = type switch
        {
            LiteralEnum.Decimal => Expression.Constant(double.Parse(literalString, NumberStyles.Float, NumberFormatInfo.InvariantInfo),
                typeof( double? )),
            _ => throw new Exception("Invalid AST: Invalid type of literal!")
        };
        return expression;
    }

    public Expression WrapParameter(ParameterExpression parameterExpression)
    {
        return _expressionProperties is not { UseFallbacks: true } ? parameterExpression.Current() : parameterExpression.Latest();
    }

    public bool IsParameterTypeSupported(Type type)
    {
        return type == typeof( TagData<double> );
    }

    public ParameterExpression BuildParameter(string name, Type type)
    {
        return Expression.Parameter(type, name);
    }

    public LambdaExpression BuildLambda(Expression body, IEnumerable<ParameterExpression> parameters)
    {
        return Expression.Lambda(body, parameters);
    }
    
    public Expression WrapExpression(Expression expression)
    {
        return Expression.New(typeof( TagData<double> ).GetConstructors()[0], expression, Expression.Constant(false));
    }
}
