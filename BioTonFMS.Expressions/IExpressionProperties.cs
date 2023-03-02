namespace BioTonFMS.Expressions;

public interface IExpressionProperties
{
    string Name { get; }
    string Formula { get; }
    bool UseFallbacks { get; }
}
