namespace BioTonFMS.Expressions.AST;

public class BinaryOperation : AstNode, IEquatable<BinaryOperation>
{
    public AstNode LeftNode { get; }
    public AstNode RightNode { get; }
    public BinaryOperationEnum Operation { get; }

    public BinaryOperation(AstNode leftNode, AstNode rightNode, BinaryOperationEnum operation)
    {
        LeftNode = leftNode;
        RightNode = rightNode;
        Operation = operation;
    }
    public bool Equals(BinaryOperation? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return LeftNode.Equals(other.LeftNode) && RightNode.Equals(other.RightNode) && Operation == other.Operation;
    }
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        return obj.GetType() == GetType() && Equals((BinaryOperation)obj);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(LeftNode, RightNode, (int)Operation);
    }
    public static bool operator ==(BinaryOperation? left, BinaryOperation? right)
    {
        return Equals(left, right);
    }
    public static bool operator !=(BinaryOperation? left, BinaryOperation? right)
    {
        return !Equals(left, right);
    }

    public override string ToString()
    {
        return $"({LeftNode} {Operation} {RightNode})";
    }
}
