using System.Linq.Expressions;
using BioTonFMS.Expressions.AST;

namespace BioTonFMS.Expressions;

internal class DefaultExecutionHandler : IExecutionHandler
{
    public AstNode? ExecuteParsing(Func<AstNode?> compileFunc) => compileFunc();
    public Expression? ExecuteCompilation(Func<Expression?> parseFunc) => parseFunc();
}
