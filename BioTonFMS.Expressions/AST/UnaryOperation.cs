namespace BioTonFMS.Expressions.AST;

public class UnaryOperation : AstNode, IEquatable<UnaryOperation>
{
    public AstNode Node { get; }
    public UnaryOperationEnum Operation { get; }

    public UnaryOperation(AstNode node, UnaryOperationEnum operation)
    {
        Node = node;
        Operation = operation;
    }
    public bool Equals(UnaryOperation? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Node.Equals(other.Node) && Operation == other.Operation;
    }
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        return obj.GetType() == GetType() && Equals((UnaryOperation)obj);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Node, (int)Operation);
    }
    public static bool operator ==(UnaryOperation? left, UnaryOperation? right)
    {
        return Equals(left, right);
    }
    public static bool operator !=(UnaryOperation? left, UnaryOperation? right)
    {
        return !Equals(left, right);
    }
}