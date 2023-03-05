using BioTonFMS.Expressions.Ast;

namespace BioTonFMS.Expressions.Compilation;

internal static class CompilerAstExtensions
{
    /// <summary>
    /// Traverses AST and extracts names of variables
    /// </summary>
    /// <param name="node">AST root</param>
    /// <returns>Set of variable names</returns>
    public static IEnumerable<Variable> GetVariables(this AstNode node)
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
                case FunctionCall v:
                    var arguments = v.Arguments;
                    if (arguments.Length == 0)
                        continue;
                    node = arguments[0];
                    for (var i = 1; i < arguments.Length; i++)
                        stack.Push(arguments[i]);
                    continue;
                case UnaryOperation v:
                    node = v.Operand;
                    continue;
                case Variable v:
                    yield return v;
                    break;
            }
            if (stack.Count == 0)
                break;
            node = stack.Pop();
        }
    }
}
