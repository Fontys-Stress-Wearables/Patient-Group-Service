using Patient_Group_Service.Events;
using Patient_Group_Service.Exceptions;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Services;

public class PatientGroupService : IPatientGroupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INatsService _natsService;

    private readonly ICaregiverService _caregiverService;
    private readonly IPatientService _patientService;

    public PatientGroupService(IUnitOfWork unitOfWork, INatsService natsService, ICaregiverService caregiverService, IPatientService patientService)
    {
        _unitOfWork = unitOfWork;
        _natsService = natsService;
        _caregiverService = caregiverService;
        _patientService = patientService;
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

    public PatientGroup Update(string patientGroupId, string? name, string? description, string tenantId)
    {
        var group = _unitOfWork.PatientGroups.GetByIdAndTenant(patientGroupId, tenantId);

        if (group == null)
        {
            throw new NotFoundException($"Patient group with id '{patientGroupId}' doesn't exist.");
        }

        if (name != null) group.GroupName = name;
        if (description != null) group.Description = description;
        
        var updated = _unitOfWork.PatientGroups.Update(group);
        
        _natsService.Publish("patient-group-updated", tenantId, new PatientGroupUpdatedEvent{GroupId = updated.Id, Name = updated.GroupName, Description = updated.Description});
        _natsService.Publish("th-logs","", $"Patient-Group updated with ID: '{updated.Id}.'");
        _unitOfWork.Complete();

        return updated;
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

        var patientGroup = new PatientGroup
        {
            Id = Guid.NewGuid().ToString(),
            GroupName = name,
            Description = description,
            Organization = org
        };

        _unitOfWork.PatientGroups.Add(patientGroup);

        _natsService.Publish("patient-group-created", tenantId, new PatientGroupCreatedEvent{GroupId = patientGroup.Id, Name = patientGroup.GroupName, Description = patientGroup.Description, OrganizationId = tenantId});
        _natsService.Publish("th-logs","", $"Patient-Group created with ID: '{patientGroup.Id}.'");
        _unitOfWork.Complete();

        return patientGroup;
    }

    public async Task<IEnumerable<PatientGroup>> GetForCaregiver(string caregiverId, string tenantId)
    {
        var caregiver = await _caregiverService.Get(caregiverId, tenantId);
        
        if(caregiver == null)
        {
            throw new NotFoundException($"Caregiver with id '{caregiverId}' doesn't exist.");
        }

        return caregiver.PatientGroupCaregivers.Select(x => x.PatientGroup);
    }

    public void Delete(string id, string tenantId)
    {
        var group = _unitOfWork.PatientGroups.GetByIdAndTenant(id, tenantId);

        if (group == null)
        {
            throw new NotFoundException($"Patient group with id '{id}' doesn't exist.");
        }

        _unitOfWork.PatientGroups.Remove(group);
        
        _natsService.Publish("patient-group-removed", tenantId, new PatientGroupRemovedEvent{GroupId = id});
        _natsService.Publish("th-logs","", $"Patient-Group removed with ID: '{id}.'");

        _unitOfWork.Complete();
    }

    public void AddPatient(string patientGroupId, string patientId, string tenantId)
    {
        var patientGroup = Get(patientGroupId, tenantId);
        
        var patient = _patientService.Get(patientId, tenantId);

        _unitOfWork.PatientGroups.AddPatient(patientGroup, patient);

        _natsService.Publish("patient-group-patient-added", tenantId,new PatientAddedEvent{GroupId = patientGroupId, PatientId = patientId});
        _natsService.Publish("th-logs","", $"Patient added to the Patient-Group with ID: '{patientGroup.Id}.'");
        _unitOfWork.Complete();
    }

    public void RemovePatient(string patientGroupId, string patientId, string tenantId)
    {
        var patientGroup = Get(patientGroupId, tenantId);

        var patientGroupPatient = _unitOfWork.PatientGroups.GetPatientGroupPatient(patientGroup, patientId);
        if (patientGroupPatient == null)
        {
            throw new BadRequestException($"Patient with id '{patientId}' could not be removed from patient group with id:'{patientGroupId}'.");
        }
        _unitOfWork.PatientGroups.RemovePatient(patientGroupPatient);
        
        _natsService.Publish("patient-group-patient-removed", tenantId, new PatientRemovedEvent{GroupId = patientGroupId, PatientId = patientId});
        _natsService.Publish("th-logs","", $"Patient removed from the Patient-Group with ID: '{patientGroup.Id}.'");
        _unitOfWork.Complete();
    }

    public async Task AddCaregiver(string patientGroupId, string caregiverId, string tenantId)
    {
        var patientGroup = Get(patientGroupId, tenantId);

        var caregiver = await _caregiverService.Get(caregiverId, tenantId);

        _unitOfWork.PatientGroups.AddCaregiver(patientGroup, caregiver);

        _natsService.Publish("patient-group-caregiver-added", tenantId, new CaregiverAddedEvent{GroupId = patientGroupId, CaregiverId = caregiverId});
        _natsService.Publish("th-logs","", $"Caregiver added to the Patient-Group with ID: '{patientGroup.Id}.'");
        _unitOfWork.Complete();
    }
    
    public async Task RemoveCaregiver(string patientGroupId, string caregiverId, string tenantId)
    {
        var caregiver = await _caregiverService.Get(caregiverId, tenantId);
        
        var patientGroup = Get(patientGroupId, tenantId);

        var patientGroupCaregiver = _unitOfWork.PatientGroups.GetPatientGroupCaregiver(patientGroup, caregiver.Id);
        if (patientGroupCaregiver == null)
        {
            throw new BadRequestException($"Caregiver with id '{caregiverId}' could not be removed from patient group with id:'{patientGroupId}'.");
        }
        _unitOfWork.PatientGroups.RemoveCaregiver(patientGroupCaregiver);
        
        _natsService.Publish("patient-group-caregiver-removed", tenantId, new CaregiverRemovedEvent{GroupId = patientGroupId, CaregiverId = caregiverId});
        _natsService.Publish("th-logs","", $"Caregiver removed from the Patient-Group with ID: '{patientGroup.Id}.'");
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

    public IEnumerable<PatientGroup> GetForPatient(string patientId, string tenantId)
    {
        var patient = _patientService.Get(patientId, tenantId);
        
        return patient.PatientGroupPatients.Select(pg => pg.PatientGroup);
    }

    public IEnumerable<Caregiver> GetCaregivers(string id, string tenantId)
    {
        var patientGroup = Get(id, tenantId);
        return patientGroup.PatientGroupCaregivers.Select(pg => pg.Caregiver);
    }
}