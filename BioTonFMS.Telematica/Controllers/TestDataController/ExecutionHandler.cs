using System.Linq.Expressions;
using BioTonFMS.Expressions;
using BioTonFMS.Expressions.AST;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Controllers.TestData;

public class ExecutionHandler : IExecutionHandler
{
    private readonly ILogger _logger;
        
    public ExecutionHandler(ILogger logger)
    {
        _logger = logger;
    }

    public AstNode? ExecuteParsing(Func<AstNode?> parseFunc)
    {
        try
        {
            return parseFunc();
        }
        catch( Exception e )
        {
            _logger.Log(LogLevel.Error, e, "Exception while parsing!");
            return null;
        }
    }
    public Expression? ExecuteCompilation(Func<Expression?> compileFunc)
    {
        try
        {
            return compileFunc();
        }
        catch( Exception e )
        {
            _logger.Log(LogLevel.Error, e, "Exception while compiling!");
            return null;
        }
    }
}
