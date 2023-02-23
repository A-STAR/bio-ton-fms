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
                (Name: "c", (Formula: "a", UseFallBack: false)),
                (Name: "d", (Formula: "b", UseFallBack: false))
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
        result[0].Item1.Should().Be("c");
        result[0].Item2.Should().NotBeNull();
        result[0].Item2!.ToString().Should().Be("a => IIF(a.IsFallback, null, a.Value)");
        result[1].Item1.Should().Be("d");
        result[1].Item2.Should().NotBeNull();
        result[1].Item2!.ToString().Should().Be("b => IIF(b.IsFallback, null, b.Value)");
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
        var lambdas = new List<(string, Expression?)>
        {
            (
                "b",
                Expression.Lambda(parameter, parameters)
            )
        };
        var result = lambdas.Execute(arguments).ToArray();
        result.Length.Should().Be(1);
        result[0].Item1.Should().Be("b");
        result[0].Item2.Should().BeOfType(typeof( double ));
        result[0].Item2.Should().Be(222.33);
    }
}
