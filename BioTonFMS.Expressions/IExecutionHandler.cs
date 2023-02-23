using System.Linq.Expressions;
using BioTonFMS.Expressions.AST;

namespace BioTonFMS.Expressions;

public interface IExecutionHandler
{
    AstNode? ExecuteParsing(Func<AstNode?> parseFunc);
    Expression? ExecuteCompilation(Func<Expression?> compileFunc);
}
