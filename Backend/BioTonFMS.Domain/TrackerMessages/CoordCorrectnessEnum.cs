namespace BioTonFMS.Domain.TrackerMessages;

public enum CoordCorrectnessEnum
{
    /// <summary>
    /// Координаты корректны, источник ГЛОНАСС/GPS модуль
    /// </summary>
    CorrectGps = 0,
    
    /// <summary>
    /// Координаты корректны, источник базовые станции сотовой сети,
    /// </summary>
    CorrectGsm = 2,
    
    /// <summary>
    /// Некорректные
    /// </summary>
    Incorrect = 3
}