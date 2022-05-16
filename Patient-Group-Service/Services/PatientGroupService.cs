using Microsoft.Graph;
using Microsoft.Identity.Web;
using Patient_Group_Service.Events;
using Patient_Group_Service.Exceptions;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Services;

public class PatientGroupService : IPatientGroupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INatsService _natsService;
    private readonly IConfiguration _configuration;
    private readonly ICaregiverService _caregiverService;
        
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly GraphServiceClient _graphServiceClient;

    public PatientGroupService(IUnitOfWork unitOfWork, INatsService natsService, ITokenAcquisition tokenAcquisition, GraphServiceClient graphServiceClient, IConfiguration configuration, ICaregiverService caregiverService)
    {
        _unitOfWork = unitOfWork;
        _natsService = natsService;
        _tokenAcquisition = tokenAcquisition;
        _graphServiceClient = graphServiceClient;
        _configuration = configuration;
        _caregiverService = caregiverService;
    }

    public PatientGroup Get(string patientGroupId, string tenantId)
    {
        var group = _unitOfWork.PatientGroups.GetByIdAndTenant(patientGroupId, tenantId);

        if (group == null)
        {
            throw new NotFoundException($"Patient group with id '{patientGroupId}' doesn't exist.");
        }

        return group;
    }

    public PatientGroup Create(string name, string? description, string tenantId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BadRequestException("Name cannot be empty.");
        }

        var org = _unitOfWork.Organizations.GetById(tenantId);
        
        if (org == null)
        {
            throw new NotFoundException($"Organization with id '{tenantId}' doesn't exist.");
        }

        var pg = new PatientGroup
        {
            Id = Guid.NewGuid().ToString(),
            GroupName = name,
            Description = description,
            Organization = org
        };

        _unitOfWork.PatientGroups.Add(pg);

        _natsService.Publish("patient-group-created", new PatientGroupCreatedEvent{GroupId = pg.Id, Name = pg.GroupName, Description = pg.Description, OrganizationId = tenantId});

        _unitOfWork.Complete();

        return pg;
    }

    public void AddPatient(string patientGroupId, string patientId, string tenantId)
    {
        var patientGroup = Get(patientGroupId, tenantId);
        
        var patient = _unitOfWork.Patients.GetByIdAndTenant(patientId, tenantId);

        if (patient == null)
        {
            throw new BadRequestException($"Patient with id '{patientId}' doesn't exist.");
        }

        _unitOfWork.PatientGroups.AddPatient(patientGroup, patient);

        _natsService.Publish("patient-group-patient-added", new PatientAddedEvent{GroupId = patientGroupId, PatientId = patientId});

        _unitOfWork.Complete();
    }

    public void RemovePatientFromPatientGroup(string patientGroupId, string patientId, string tenantId)
    {
        var patient = _unitOfWork.Patients.GetByIdAndTenant(patientId, tenantId);
        
        var patientGroup = Get(patientGroupId, tenantId);
        
        if (patient == null)
        {
            throw new BadRequestException($"Patient with id '{patientId}' doesn't exist.");
        }

        var patientGroupPatient = _unitOfWork.PatientGroups.GetPatientGroupPatient(patientGroup, patient);
        if (patientGroupPatient == null)
        {
            throw new BadRequestException($"Patient with id '{patientId}' could not be removed from patient group with id:'{patientGroupId}'.");
        }
        _unitOfWork.PatientGroups.RemovePatient(patientGroupPatient);
        
        _natsService.Publish("patient-group-patient-removed", new PatientRemovedEvent{GroupId = patientGroupId, PatientId = patientId});

        _unitOfWork.Complete();
    }

    public async Task AddCaregiver(string patientGroupId, string caregiverId, string tenantId)
    {
        var patientGroup = Get(patientGroupId, tenantId);

        var caregiver = _caregiverService.Get(caregiverId, tenantId);
        
        if (caregiver == null)
        {
            var scopesToAccessDownstreamApi = new[] { _configuration["AzureAD:GraphScope"] };

            await _tokenAcquisition.GetAccessTokenForUserAsync(scopesToAccessDownstreamApi);
            var users = await _graphServiceClient.Users
                .Request()
                .GetAsync();

            if (users.Any(x => x.Id == caregiverId))
            {
                var cg = new Caregiver {Id = caregiverId};
                caregiver = _unitOfWork.Caregivers.Add(cg);
                _unitOfWork.Complete();
            }
            else
            {
                throw new CouldNotCreateException($"Caregiver not add found in azureAD with id {caregiverId}");
            }
            
            if (caregiver == null)
            {
                throw new CouldNotCreateException($"Could not add caregiver {caregiverId}");
            }
        }

        _unitOfWork.PatientGroups.AddCaregiver(patientGroup, caregiver);

        _natsService.Publish("patient-group-caregiver-added", new CaregiverAddedEvent{GroupId = patientGroupId, CaregiverId = caregiverId});

        _unitOfWork.Complete();
    }

    public IEnumerable<PatientGroup> GetAll(string tenantId)
    {
        return _unitOfWork.PatientGroups.GetAllFromTenant(tenantId);
    }

    public IEnumerable<Patient> GetPatients(string id, string tenantId)
    {
        var patientGroup = Get(id, tenantId);
        return patientGroup.PatientGroupPatients.Select(pg => pg.Patient);
    }

    public IEnumerable<Caregiver> GetCaregivers(string id, string tenantId)
    {
        var patientGroup = Get(id, tenantId);
        return patientGroup.PatientGroupCaregivers.Select(pg => pg.Caregiver);
    }

    public void Delete(string id, string tenantId)
    {
        var group = _unitOfWork.PatientGroups.GetByIdAndTenant(id, tenantId);

        if (group == null)
        {
            throw new NotFoundException($"Patient group with id '{id}' doesn't exist.");
        }

        _unitOfWork.PatientGroups.Remove(group);
        
        _natsService.Publish("patient-group-removed", new PatientGroupRemovedEvent{GroupId = id});
        
        _unitOfWork.Complete();
    }
}