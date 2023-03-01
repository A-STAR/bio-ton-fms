using System.Linq.Expressions;
using BioTonFMS.Expressions;
using FluentAssertions;

namespace BiotonFMS.Telematica.Tests.Expressions;

public class HelpersTests
{
    struct TestExpressionProperties : Helpers.IExpressionProperties
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

    [Fact]
    public void SortAndBuild_CorrectInput_CorrectOutput()
    {
        var result =
            new[]
            {
                new TestExpressionProperties(name: "c", formula: "d", useFallbacks: false),
                new TestExpressionProperties(name: "d", formula: "b", useFallbacks: false),
            }.SortAndBuild(new Dictionary<string, Type>()
            {
                {
                    "a", typeof( TagData<double> )
                },
                {
                    "b", typeof( TagData<double> )
                },
            }).ToArray();
        result.Length.Should().Be(2);

        result[0].Properties.Name.Should().Be("d");
        result[0].Properties.Formula.Should().Be("b");
        result[0].Properties.UseFallbacks.Should().BeFalse();
        result[0].ExpressionTree.Should().NotBeNull();
        TestUtil.ExtractUnwrappedExpression(result[0].ExpressionTree)!.ToString().Should().Be("IIF(b.IsFallback, null, b.Value)");
        
        result[1].Properties.Name.Should().Be("c");
        result[1].Properties.Formula.Should().Be("d");
        result[1].Properties.UseFallbacks.Should().BeFalse();
        result[1].ExpressionTree.Should().NotBeNull();
        TestUtil.ExtractUnwrappedExpression(result[1].ExpressionTree)!.ToString().Should().Be("IIF(d.IsFallback, null, d.Value)");
    }

    [Fact]
    public void ExecuteSingle_CorrectLambda_CorrectResult()
    {
        var param = Expression.Parameter(typeof( double ), "a");
        var parameters = new[]
        {
            param
        };
        var arguments = new Dictionary<string, object?>()
        {
            {
                "a", 222.33
            }
        };
        var result = Helpers.Execute(Expression.Lambda(param, parameters), arguments);
        result.Should().BeOfType(typeof( double ));
        result.Should().Be(222.33);
    }

    [Fact]
    public void ExecuteSequence()
    {
        var parameter = Expression.Parameter(typeof( double ), "a");
        var parameters = new[]
        {
            parameter
        };
        var arguments = new Dictionary<string, object?>()
        {
            {
                "a", 222.33
            }
        };
        var lambdas = new List<CompiledExpression<TestExpressionProperties>>
        {
            new CompiledExpression<TestExpressionProperties>(
                new TestExpressionProperties("b", "f", false),
                Expression.Lambda(parameter, parameters)
                )
        };
        var result = lambdas.Execute(arguments).ToArray();
        result.Length.Should().Be(1);
        result[0].Item1.Name.Should().Be("b");
        result[0].Item1.Formula.Should().Be("f");
        result[0].Item1.UseFallbacks.Should().BeFalse();
        result[0].Item2.Should().BeOfType(typeof( double ));
        result[0].Item2.Should().Be(222.33);
    }
}
