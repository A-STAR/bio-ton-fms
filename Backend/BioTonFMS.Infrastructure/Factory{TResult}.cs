namespace BioTonFMS.Infrastructure
{
    /// <summary>
    /// Инкапсулирует метод-фабрику и возвращает созданный объект типа TResult.
    /// </summary>
    /// <typeparam name="TResult">Тип создаваемого объекта.</typeparam>
    /// <returns>Созданный объект.</returns>
    public delegate TResult Factory<out TResult>();
}