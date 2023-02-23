using System.Diagnostics;
using System.Linq.Expressions;
using BioTonFMS.Expressions.AST;

namespace BioTonFMS.Expressions;

static internal class AstExtensions
{
    public static IEnumerable<string> GetVariables(this AstNode node)
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

    public static Expression? CompileWithHandler(this AstNode node, Compiler compiler, IDictionary<string, Type> parameters,
        Func<Func<Expression?>, Expression?> executionHandler)
    {
        return executionHandler(() => compiler.Compile(node, parameters));
    }
}
