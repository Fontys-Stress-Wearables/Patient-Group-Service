using Patient_Group_Service.Interfaces;

namespace Patient_Group_Service.Services;

public class HeartBeatService : IHostedService, IDisposable
{
    private readonly INatsService _natsService;

    private Timer? _timer;
    private readonly TimeSpan _heartBeatInterval = TimeSpan.FromSeconds(30);

    public HeartBeatService(INatsService natsService)
    {
        _natsService = natsService;
    }

    private void HeartbeatTimerCallback(object? state)
    {
        _natsService.Publish("technical_health", "", "heartbeat");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(HeartbeatTimerCallback, null, TimeSpan.Zero, _heartBeatInterval);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
        GC.SuppressFinalize(this);
    }
}