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
        
        _natsService.Subscribe<Organization>("organization-created", OnOrganizationCreated);
        _natsService.Subscribe<Organization>("organization-removed", OnOrganizationRemoved);
        
        return Task.CompletedTask;
    }
    
    private void OnOrganizationRemoved(NatsMessage<Organization> message)
    {
        using var scope = _services.CreateScope();

        var scopedOrganizationService = 
            scope.ServiceProvider
                .GetRequiredService<IOrganizationService>();
        
        scopedOrganizationService.Remove(message.message);
    }
    
    private void OnOrganizationCreated(NatsMessage<Organization> message)
    {
        using var scope = _services.CreateScope();

        var scopedOrganizationService = 
            scope.ServiceProvider
                .GetRequiredService<IOrganizationService>();
        
        scopedOrganizationService.Create(message.message);
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