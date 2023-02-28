using System.Linq.Expressions;
using BioTonFMS.Expressions;
using BioTonFMS.Expressions.AST;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.Telematica.Controllers.TestData;

public class ExceptionHandler : IExceptionHandler
{
    private readonly ILogger _logger;

    public ExceptionHandler(ILogger logger)
    {
        _logger = logger;
    }

    public bool Handle(Exception exception, OperationTypeEnum type)
    {
        _logger.Log(LogLevel.Error, exception,
            type switch
            {
                OperationTypeEnum.Parsing => "Exception while parsing!",
                OperationTypeEnum.Compilation => "Exception while compiling!",
                OperationTypeEnum.Execution => "Exception during execution!",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported type of operation")
            });
        return true;
    }
}