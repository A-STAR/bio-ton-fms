using System.Diagnostics;
using System.Linq.Expressions;
using BioTonFMS.Expressions.AST;

namespace BioTonFMS.Expressions;

static internal class AstExtensions
{
    /// <summary>
    /// Traverses AST and extracts names of variables
    /// </summary>
    /// <param name="node">AST root</param>
    /// <returns>Set of variable names</returns>
    public static IEnumerable<string> GetVariables(this AstNode node)
    {
        var stack = new Stack<AstNode>(8);
        while (true)
        {
            switch (node)
            {
                case BinaryOperation v:
                    node = v.LeftOperand;
                    stack.Push(v.RightOperand);
                    continue;
                case UnaryOperation v:
                    node = v.Operand;
                    continue;
                case Variable v:
                    yield return v.Name;
                    break;
            }
            if (stack.Count == 0)
                break;
            node = stack.Pop();
        }
    }

    /// <summary>
    /// Compiles expression while using passed exception handler to handle exceptions
    /// </summary>
    /// <param name="node">Root of AST to compile</param>
    /// <param name="compiler">Compiler used to compile the AST</param>
    /// <param name="parameters">Names and types of available input parameters.</param>
    /// <param name="exceptionHandler">Object which handles exception thrown during parsing, compiling or execution.</param>
    /// <returns>Expression tree which is the result of compilation of AST</returns>
    public static Expression? CompileWithHandler(this AstNode node, Compiler compiler, IDictionary<string, Type> parameters, IExceptionHandler exceptionHandler)
    {
        try
        {
            return compiler.Compile(node, parameters);
        }
        catch( Exception e )
        {
            if (!exceptionHandler.Handle(e, OperationTypeEnum.Parsing))
                throw;
        }
        return null;
    }
}
