namespace BioTonFMS.Infrastructure
{
    /// <summary>
    /// Инкапсулирует метод-фабрику с одним аргументом и возвращает созданный объект типа TResult.
    /// </summary>
    /// <typeparam name="T">Тип аргумента.</typeparam>
    /// <typeparam name="TResult">Тип создаваемого объекта.</typeparam>
    /// <param name="arg">Аргумент метода-фабрики.</param>
    /// <returns>Созданный объект.</returns>
    public delegate TResult Factory<in T, out TResult>(T arg);
}