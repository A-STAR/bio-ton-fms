using BioTonFMS.Domain;
using BioTonFMS.Expressions;

namespace BioTonFMS.MessageProcessing;

public interface ISensorExpressionProperties : IExpressionProperties
{
    /// <summary>
    /// If it is true then in cases when arguments passed to the executed expression represent fallback values
    /// (values taken from previous messages) expression will use those values, and if this option is false
    /// it will replace those values with nulls 
    /// </summary>
    bool UseFallbacks { get; }
    
    /// <summary>
    /// Type of "validator"
    /// </summary>
    ValidationTypeEnum? ValidationType { get; }
    
    /// <summary>
    /// Parameter name of source of values for "validator"
    /// </summary>
    string? ValidatorName { get; }
    
}
