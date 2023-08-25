namespace BioTonFMS.Expressions;

/// <summary>
/// Свойства выражения
/// </summary>
public interface IExpressionProperties
{
    /// <summary>
    /// Имя выражения, на которое могут ссылаться другие выражения.
    /// Пример: { "a": "const1", "b": "a + a" }, где формат: { &lt;имя&gt;: &lt;выражение&gt;, ... }
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Выражение представленное в виде строки
    /// </summary>
    string Formula { get; }
}
