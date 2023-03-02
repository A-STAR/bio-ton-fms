namespace BioTonFMS.Expressions.AST;

/// <summary>
/// Type of AST nodes of binary arithmetic operation 
/// </summary>
public class BinaryOperation : AstNode, IEquatable<BinaryOperation>
{
    /// <summary>
    /// Left operand of binary operation
    /// </summary>
    public AstNode LeftOperand { get; }
    
    /// <summary>
    /// Right operand of binary operation
    /// </summary>
    public AstNode RightOperand { get; }
    
    /// <summary>
    /// Type of binary operation
    /// </summary>
    public BinaryOperationEnum Operation { get; }

    public BinaryOperation(AstNode leftOperand, AstNode rightOperand, BinaryOperationEnum operation)
    {
        LeftOperand = leftOperand;
        RightOperand = rightOperand;
        Operation = operation;
    }
    public bool Equals(BinaryOperation? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return LeftOperand.Equals(other.LeftOperand) && RightOperand.Equals(other.RightOperand) && Operation == other.Operation;
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
        return HashCode.Combine(LeftOperand, RightOperand, (int)Operation);
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
        return $"({LeftOperand} {Operation} {RightOperand})";
    }
}
