namespace BioTonFMS.Expressions.Compilation;

public class CompilationException : Exception
{

    public ICollection<CompilationError> CompilationErrors { get; }

    public CompilationException(string message, ICollection<CompilationError> compilationErrors) : base(message)
    {
        CompilationErrors = compilationErrors;
    }
}
