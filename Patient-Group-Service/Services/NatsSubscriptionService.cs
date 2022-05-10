

using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Services;

public class NatsSubscriptionService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly INatsService _natsService;
    
    public NatsSubscriptionService(IServiceProvider services, INatsService natsService)
    {
        _services = services;
        _natsService = natsService;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _natsService.Subscribe<Patient>("patient-created", OnPatientCreated);
        return Task.CompletedTask;
    }

    private void OnPatientCreated(NatsMessage<Patient> message)
    {
        using var scope = _services.CreateScope();
        
        var scopedPatientService = 
            scope.ServiceProvider
                .GetRequiredService<IPatientService>();
        
        scopedPatientService.Create(message.message);
    }
}