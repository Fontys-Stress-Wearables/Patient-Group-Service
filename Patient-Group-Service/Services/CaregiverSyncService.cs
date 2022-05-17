using Patient_Group_Service.Interfaces;

namespace Patient_Group_Service.Services;

public class CaregiverSyncService : IHostedService, IDisposable
{
    private readonly INatsService _natsService;
    private readonly IServiceProvider _serviceProvider;
    
    private Timer _timer = null!;
    
    public CaregiverSyncService(INatsService natsService, IServiceProvider serviceProvider)
    {
        _natsService = natsService;
        _serviceProvider = serviceProvider;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(SyncCaregivers, null, 1000, 100000);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
    
    private void SyncCaregivers(object? state)
    {
        _ = DoSyncCaregivers();
    }

    private async Task DoSyncCaregivers()
    {
        using var scope = _serviceProvider.CreateScope();
        var scopedOrganizationService = 
            scope.ServiceProvider
                .GetRequiredService<IOrganizationService>();

        var organizations = scopedOrganizationService.GetAll();
        
        foreach (var organization in organizations)
        {
            try
            {
                await FetchCaregivers(organization.Id);
            }
            catch (Exception ex)
            {
                _natsService.Publish("th_errors", ex.Message);
            }
        }
    }

    private async Task FetchCaregivers(string tenantId)
    {
        using var scope = _serviceProvider.CreateScope();
        var scopedCaregiverService = 
            scope.ServiceProvider
                .GetRequiredService<ICaregiverService>();

        await scopedCaregiverService.FetchFromGraph(tenantId);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _timer.Dispose();
    }
}