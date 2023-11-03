using System.ComponentModel.DataAnnotations;

namespace BioTonFMS.Telematica.Dtos.MessagesView;

/// <summary>
/// Модель машины для экрана просмотра сообщений
/// </summary>
public class MessagesViewVehicleDto
{
    /// <summary>
    /// Id машины
    /// </summary>
    [Required]
    public int Id { get; set; }

    /// <summary>
    /// Наименование машины
    /// </summary>
    [Required]
    public string Name { get; set; } = "";
}
