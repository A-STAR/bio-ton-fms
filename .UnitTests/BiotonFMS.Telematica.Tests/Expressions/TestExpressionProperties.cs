using BioTonFMS.Expressions;

namespace BiotonFMS.Telematica.Tests.Expressions;

internal struct TestExpressionProperties : IExpressionProperties
{
    public string Name { get; }
    public string Formula { get; }
    public bool UseFallbacks { get; }

    public TestExpressionProperties(string name, string formula, bool useFallbacks)
    {
        Name = name;
        Formula = formula;
        UseFallbacks = useFallbacks;
    }
}
