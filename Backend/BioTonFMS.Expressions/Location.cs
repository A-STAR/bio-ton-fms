namespace BioTonFMS.Expressions;

public readonly struct Location
{
    public readonly int Start;
    public readonly int Length;
    
    public Location(int start, int length = 0)
    {
        Start = start;
        Length = length;
    }
}
