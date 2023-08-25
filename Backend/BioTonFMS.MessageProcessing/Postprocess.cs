using BioTonFMS.Domain;
using BioTonFMS.Expressions.Ast;

namespace BioTonFMS.MessageProcessing;

public static class Postprocess
{
    /// <summary>
    /// Выполняет постобработку AST датчика, добавляя в него вычисление "валидатора",
    /// при его наличии.
    /// </summary>
    /// <param name="ast">Дерево выражения датчика</param>
    /// <param name="properties">Свойства выражения датчика</param>
    /// <returns>Обработанное выражение с добавленным "валидатором"</returns>
    /// <exception cref="Exception">Исключение выбрасывается в том случае, если тип датчика неверен</exception>
    public static AstNode? AddSensorValidatorToAst(AstNode? ast, SensorExpressionProperties properties)
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
