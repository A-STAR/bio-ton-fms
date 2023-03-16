using System.Linq.Expressions;
using BioTonFMS.Expressions.Ast;

namespace BioTonFMS.Expressions.Compilation;

public interface IExpressionBuilder
{
    /// <summary>
    /// Builds unary operation of specified type
    /// </summary>
    /// <param name="operation">Type of operation to build</param>
    /// <param name="operand">Operand of the operation</param>
    /// <returns>Expression tree of built operation</returns>
    Expression BuildUnary(UnaryOperationEnum operation, Expression operand);

    /// <summary>
    /// Builds binary operation of specified type
    /// </summary>
    /// <param name="operation">Type of operation to build</param>
    /// <param name="leftOperand">Left operand of the operation</param>
    /// <param name="rightOperand">Right operand of the operation</param>
    /// <returns>Expression tree of built operation</returns>
    Expression BuildBinary(BinaryOperationEnum operation, Expression leftOperand, Expression rightOperand);

    /// <summary>
    /// Builds function with given name and arguments
    /// </summary>
    /// <param name="name">Name of function to build</param>
    /// <param name="arguments">Arguments of the function</param>
    /// <returns>Expression tree of built function</returns>
    Expression BuildFunction(string name, Expression[] arguments);
    
    /// <summary>
    /// Builds constant of specified type
    /// </summary>
    /// <param name="type">Type of literal for which we build constant</param>
    /// <param name="literalString">Literal</param>
    /// <returns>Expression tree of built operation</returns>
    Expression BuildConstant(LiteralEnum type, string literalString);
    
    /// <summary>
    /// Wraps parameter expression if it needs additional processing before
    /// being used in other expressions
    /// </summary>
    /// <param name="parameterExpression">Parameter expression to wrap</param>
    /// <returns>Wrapped parameter expression</returns>
    Expression WrapParameter(ParameterExpression parameterExpression);

    /// <summary>
    /// Is called by compiler to validate types of parameters
    /// </summary>
    /// <param name="type">Type to check for compatibility</param>
    /// <returns>Returns true if type is supported</returns>
    bool IsParameterTypeSupported(Type type);
    
    /// <summary>
    /// Builds parameter expression
    /// </summary>
    /// <param name="name">Name of parameter</param>
    /// <param name="type">Type of parameter</param>
    /// <returns>Parameter expression</returns>
    ParameterExpression BuildParameter(string name, Type type);

    /// <summary>
    /// Builds lambda expression used to wrap entire expression into executable package
    /// </summary>
    /// <param name="body">Body of lambda</param>
    /// <param name="parameters">Parameters of lambda</param>
    /// <returns>Built lambda expression</returns>
    LambdaExpression BuildLambda(Expression body, IEnumerable<ParameterExpression> parameters);

    /// <summary>
    /// May be used to wrap entire expression into some other expression in order to add post processing 
    /// </summary>
    /// <param name="expression">Entire compiled expression</param>
    /// <returns>Wrapped expression</returns>
    /// <remarks>Shouldn't be used to add dependencies on parameters because it will break dependency
    /// tracking mechanisms which rely on AST processing</remarks>
    Expression WrapExpression(Expression expression);
}