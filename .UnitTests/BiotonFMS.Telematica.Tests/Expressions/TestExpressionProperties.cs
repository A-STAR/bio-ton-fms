using BioTonFMS.Expressions;

namespace BiotonFMS.Telematica.Tests.Expressions;

internal struct TestExpressionProperties : IExpressionProperties
{
    public string Name { get; }
    public string Formula { get; }
    public bool UseFallbacks { get; }
    public bool HasValidator { get; }
    public string? ValidatorName { get; }

    public TestExpressionProperties(string name, string formula, bool useFallbacks, bool hasValidator = false, string? validatorName = null)
    {
        Name = name;
        Formula = formula;
        UseFallbacks = useFallbacks;
        HasValidator = hasValidator;
        ValidatorName = validatorName;
    }
}
