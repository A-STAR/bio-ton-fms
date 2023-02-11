namespace BioTonFMS.Expressions.AST;

public class Literal : AstNode, IEquatable<Literal>
{
    public string Value { get; }
    public LiteralEnum Type { get; }

    public Literal(string value, LiteralEnum type)
    {
        Value = value;
        Type = type;
    }
    public bool Equals(Literal? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Value == other.Value && Type == other.Type;
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
        return HashCode.Combine(Value, (int)Type);
    }
    public static bool operator ==(Literal? left, Literal? right)
    {
        return Equals(left, right);
    }
    public static bool operator !=(Literal? left, Literal? right)
    {
        return !Equals(left, right);
    }
}