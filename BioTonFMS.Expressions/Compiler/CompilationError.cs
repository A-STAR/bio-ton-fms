using BioTonFMS.Expressions.Ast;

namespace BioTonFMS.Expressions.Compilation;

public struct CompilationError
{
    public readonly ErrorType ErrorType;
    public readonly AstNode AffectedAstNode;
    public readonly Type? ParameterType;
    
    public CompilationError(ErrorType errorType, AstNode affectedAstNode, Type? parameterType)
    {
        ErrorType = errorType;
        AffectedAstNode = affectedAstNode;
        ParameterType = parameterType;
    }
}
