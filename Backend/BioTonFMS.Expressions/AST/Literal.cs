namespace BioTonFMS.Expressions.Ast;

/// <summary>
/// Type of AST nodes representing different types of literals
/// </summary>
public class Literal : AstNode, IEquatable<Literal>
{
    /// <summary>
    /// String representation of literal
    /// </summary>
    public string LiteralString { get; }

    /// <summary>
    /// Type of literal
    /// </summary>
    public LiteralEnum Type { get; }

    public Literal(string literalString, LiteralEnum type)
    {
        LiteralString = literalString;
        Type = type;
    }
    public bool Equals(Literal? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return LiteralString == other.LiteralString && Type == other.Type;
    }
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        return obj.GetType() == GetType() && Equals((Literal)obj);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(LiteralString, (int)Type);
    }
    public static bool operator ==(Literal? left, Literal? right)
    {
        return Equals(left, right);
    }
    public static bool operator !=(Literal? left, Literal? right)
    {
        return !Equals(left, right);
    }

    public override string ToString()
    {
        return LiteralString;
    }

}
