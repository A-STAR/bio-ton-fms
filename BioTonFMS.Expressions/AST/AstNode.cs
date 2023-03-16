using Pegasus.Common;

namespace BioTonFMS.Expressions.Ast;

/// <summary>
/// Base type of abstract syntax tree nodes
/// </summary>
public abstract class AstNode : ILexical
{
    public Location Location { get; private set; }

    public Cursor? EndCursor
    {
        get => null;
        set => Location = new Location(Location.Start, (value?.Location ?? 0) - Location.Start);
    }
    public Cursor? StartCursor
    {
        get => null;
        set => Location = new Location(value?.Location ?? 0, Location.Start + Location.Length - (value?.Location ?? 0));
    }
}
