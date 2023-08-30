using BioTonFMS.Telematica.Services;
using FluentScheduler;

namespace BioTonFMSApp.Scheduler;

public class MoveTestTrackerMessagesJob : IJob
{
    private readonly ILogger<MoveTestTrackerMessagesJob> _logger;
    private readonly MoveTestTrackerMessagesService _moveTestTrackerMessagesService;

    public MoveTestTrackerMessagesJob(
        ILogger<MoveTestTrackerMessagesJob> logger,
        MoveTestTrackerMessagesService moveTestTrackerMessagesService)
    {
        _logger = logger;
        _moveTestTrackerMessagesService = moveTestTrackerMessagesService;
    }

    public void Execute()
    {
        _logger.LogInformation("Запущена задача по сдвигу тестовых сообщений трекеров на Сегодня");

        _moveTestTrackerMessagesService.MoveTestTrackerMessagesForToday();

        _logger.LogInformation("Выполнена задача по сдвигу тестовых сообщений трекеров на Сегодня");
    }
}
