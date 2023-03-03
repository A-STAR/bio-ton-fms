namespace BioTonFMS.Expressions.Ast;

/// <summary>
/// Binary arithmetic operations
/// </summary>
public enum BinaryOperationEnum
{
    /// <summary>
    /// E.g., "1 + 3"
    /// </summary>
    Addition,

    /// <summary>
    /// E.g., "3 - 3"
    /// </summary>
    Subtraction,
    
    /// <summary>
    /// E.g., "2 * 2"
    /// </summary>
    Multiplication,

    /// <summary>
    /// E.g., "0 / 0"
    /// </summary>
    Division
}
