using BioTonFMS.Expressions;

namespace BioTonFMS.Telematica.Expressions;

public struct SensorProblemDescription
{
    public readonly string FieldName;
    public readonly string Message;
    public readonly Location? Location; // Используется только для формул

    public SensorProblemDescription(string fieldName, string message, Location? location = null) : this()
    {
        FieldName = fieldName;
        Message = message;
        Location = location;
    }
}
