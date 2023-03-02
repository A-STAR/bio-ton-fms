namespace BioTonFMS.Expressions;

/// <summary>
/// Handles exceptions which are thrown during expression processing
/// </summary>
public interface IExceptionHandler
{
    /// <summary>
    /// Handles exception
    /// </summary>
    /// <param name="exception">Exception to handle</param>
    /// <param name="type">Type of expression processing operation which execution was interrupted by exception</param> 
    /// <returns>If this method returns false then exception will be rethrown</returns>
    bool Handle(Exception exception, OperationTypeEnum type);
}
