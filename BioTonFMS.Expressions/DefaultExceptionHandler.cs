namespace BioTonFMS.Expressions;

/// 
internal class DefaultExceptionHandler : IExceptionHandler
{
    public bool Handle(Exception exception, OperationTypeEnum type)
    {
        return false;
    }
}
