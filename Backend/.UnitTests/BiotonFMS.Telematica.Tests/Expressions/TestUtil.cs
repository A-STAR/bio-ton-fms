using System.Linq.Expressions;

namespace BiotonFMS.Telematica.Tests.Expressions;

public static class TestUtil
{
    public static Expression? ExtractUnwrappedExpression(LambdaExpression? expression)
    {
        if (expression == null)
            return null;
        var wrapper = expression.Body as NewExpression;
        return wrapper == null ? expression.Body : wrapper.Arguments[0];
    }
    
}
