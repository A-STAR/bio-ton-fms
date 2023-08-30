using FluentScheduler;
using System.ComponentModel;

namespace BioTonFMSApp.Scheduler;

public static class Scheduler
{
    public static void Init(IServiceProvider serviceProvider)
    {
        var registry = new Registry();
        registry.Schedule<MoveTestTrackerMessagesJob>().ToRunNow();
        registry.Schedule(() =>
        {
            using var scope = serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<MoveTestTrackerMessagesJob>().Execute();
        }).ToRunEvery(1).Days().At(hours: 1, minutes: 0);
        JobManager.Initialize(registry);
    }
}
