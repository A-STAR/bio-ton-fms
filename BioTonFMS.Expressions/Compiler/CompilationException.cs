using BioTonFMS.Expressions.Ast;

namespace BioTonFMS.Expressions.Compilation;

public enum ErrorType
{
    UnsupportedTypeOfParameter,
    ParameterDoesNotExist
}

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

public class CompilationException : Exception
{

    public ICollection<CompilationError> CompilationErrors { get; }

    public CompilationException(string message, ICollection<CompilationError> compilationErrors) : base(message)
    {
        CompilationErrors = compilationErrors;
    }
}
