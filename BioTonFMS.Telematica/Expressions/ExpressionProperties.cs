using BioTonFMS.Expressions;

namespace BioTonFMS.Telematica;

public readonly struct ExpressionProperties : IExpressionProperties
{
    public string Name { get; }
    public string Formula { get; }

    public ExpressionProperties(string name, string formula) : this()
    {
        Name = name;
        Formula = formula;
    }
}
