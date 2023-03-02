using System.Linq.Expressions;
using BioTonFMS.Expressions;
using FluentAssertions;

namespace BiotonFMS.Telematica.Tests.Expressions;

public class HelpersTests
{

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
        var lambda = Expression.Lambda(param, parameters);
        var compiledExpression = new CompiledExpression<TestExpressionProperties>(new TestExpressionProperties(), lambda);
        var result = Helpers.Execute(compiledExpression, arguments);
        result.Should().BeOfType(typeof( double ));
        result.Should().Be(222.33);
    }

    [Fact]
    public void ExecuteSequence()
    {
        var parameterA = Expression.Parameter(typeof( double ), "a");
        var parameterB = Expression.Parameter(typeof( double ), "b");

        var arguments = new Dictionary<string, object?>()
        {
            {
                "a", 222.33
            }
        };

        var lambdas = new List<CompiledExpression<TestExpressionProperties>>
        {
            new(new TestExpressionProperties("b", "a + 2" /* not used */, false),
                Expression.Lambda(Expression.Add(parameterA, Expression.Constant(2d)), parameterA)),
            new(new TestExpressionProperties("c", "b + 3" /* not used */, true),
                Expression.Lambda(Expression.Add(parameterB, Expression.Constant(3d)), parameterB)),
        };

        var result = lambdas.Execute(arguments).ToArray();

        result.Length.Should().Be(2);

        result[0].Item1.Name.Should().Be("b");
        result[0].Item1.Formula.Should().Be("a + 2");
        result[0].Item1.UseFallbacks.Should().BeFalse();
        result[0].Item2.Should().BeOfType(typeof( double ));
        result[0].Item2.Should().Be(224.33);

        result[1].Item1.Name.Should().Be("c");
        result[1].Item1.Formula.Should().Be("b + 3");
        result[1].Item1.UseFallbacks.Should().BeTrue();
        result[1].Item2.Should().BeOfType(typeof( double ));
        result[1].Item2.Should().Be(227.33);
    }
}
