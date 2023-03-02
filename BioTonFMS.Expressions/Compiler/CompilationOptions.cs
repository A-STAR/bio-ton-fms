namespace BioTonFMS.Expressions;

/// <summary>
/// Options of compilation
/// </summary>
public class CompilationOptions
{
    /// <summary>
    /// If it is true then in cases when arguments passed to the executed expression represent fallback values
    /// (values taken from previous messages) expression will use those values, and if this option is false
    /// it will replace those values with nulls 
    /// </summary>
    public bool UseFallbacks { get; init; }
}
