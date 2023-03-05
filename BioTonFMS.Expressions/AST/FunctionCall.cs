namespace BioTonFMS.Expressions.Ast;

public class FunctionCall : AstNode, IEquatable<FunctionCall>
{
    /// <summary>
    /// Name of function
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Function arguments
    /// </summary>
    public AstNode[] Arguments { get; }

    public FunctionCall(string name, params AstNode[] arguments)
    {
        Name = name;
        Arguments = arguments;
    }

    public bool Equals(FunctionCall? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Name == other.Name && Arguments.Equals(other.Arguments);
    }
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
            return false;
        return Equals((FunctionCall)obj);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Arguments);
    }
    public static bool operator ==(FunctionCall? left, FunctionCall? right)
    {
        return Equals(left, right);
    }
    public static bool operator !=(FunctionCall? left, FunctionCall? right)
    {
        return !Equals(left, right);
    }
}
