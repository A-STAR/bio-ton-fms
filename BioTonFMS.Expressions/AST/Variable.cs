using BioTonFMS.Expressions.AST;

public class Variable : AstNode, IEquatable<Variable>
{
    public string Name { get; }
    
    public Variable(string name)
    {
        Name = name;
    }
    public bool Equals(Variable? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Name == other.Name;
    }
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
            return false;
        return Equals((Variable)obj);
    }
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
    public static bool operator ==(Variable? left, Variable? right)
    {
        return Equals(left, right);
    }
    public static bool operator !=(Variable? left, Variable? right)
    {
        return !Equals(left, right);
    }
    public override string ToString()
    {
        return Name;
    }

}
