using BioTonFMS.Domain;
using BioTonFMS.Expressions.Ast;
using BioTonFMS.Telematica.Expressions;

namespace BioTonFMS.Telematica;

public static class Postprocess
{
    public static AstNode? PostprocessAst(AstNode? ast, SensorExpressionProperties properties)
    {
        if (ast == null)
            return null;
        if (!properties.ValidationType.HasValue || properties.ValidatorName == null)
            return ast;

        var result = properties.ValidationType.Value switch
        {
            ValidationTypeEnum.LogicalAnd => new FunctionCall("And", ast, new Variable(properties.ValidatorName)),
            ValidationTypeEnum.LogicalOr => new FunctionCall("Or", ast, new Variable(properties.ValidatorName)),
            ValidationTypeEnum.ZeroTest => new FunctionCall("Gate", ast, new Variable(properties.ValidatorName)),
            _ => throw new Exception("Invalid type of \"validator\"!")
        };
        return result;
    }
}
