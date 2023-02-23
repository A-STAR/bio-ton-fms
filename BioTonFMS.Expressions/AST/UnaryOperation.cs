namespace BioTonFMS.Expressions.AST;

public class UnaryOperation : AstNode, IEquatable<UnaryOperation>
{
    public AstNode Operand { get; }
    public UnaryOperationEnum Operation { get; }

    public UnaryOperation(AstNode operand, UnaryOperationEnum operation)
    {
        Operand = operand;
        Operation = operation;
    }
    public bool Equals(UnaryOperation? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Operand.Equals(other.Operand) && Operation == other.Operation;
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
        return HashCode.Combine(Operand, (int)Operation);
    }
    public static bool operator ==(UnaryOperation? left, UnaryOperation? right)
    {
        return Equals(left, right);
    }
    public static bool operator !=(UnaryOperation? left, UnaryOperation? right)
    {
        return !Equals(left, right);
    }
    
    public override string ToString()
    {
        return $"{Operation} {Operand}";
    }
}