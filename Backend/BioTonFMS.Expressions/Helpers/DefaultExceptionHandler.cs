namespace BioTonFMS.Expressions;

/// <inheritdoc />
internal class DefaultExceptionHandler : IExceptionHandler
{
    /// <inheritdoc />
    public bool Handle(Exception exception, OperationTypeEnum type)
    {
        return false;
    }
}
