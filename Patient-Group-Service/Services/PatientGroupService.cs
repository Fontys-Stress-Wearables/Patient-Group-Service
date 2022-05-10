using Patient_Group_Service.Exceptions;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Services;

public class PatientGroupService : IPatientGroupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INatsService _natsService;

    public PatientGroupService(IUnitOfWork unitOfWork, INatsService natsService)
    {
        _unitOfWork = unitOfWork;
        _natsService = natsService;
    }

    public PatientGroup Get(string patientGroupId)
    {
        var group = _unitOfWork.PatientGroups.GetById(patientGroupId);

        if (group == null)
        {
            throw new NotFoundException($"Patient group with id '{patientGroupId}' doesn't exist.");
        }

        return group;
    }

    public PatientGroup Create(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BadRequestException("Name cannot be empty.");
        }

        var pg = new PatientGroup
        {
            Id = Guid.NewGuid().ToString(),
            GroupName = name,
            Description = description,
        };

        _unitOfWork.PatientGroups.Add(pg);
        _natsService.Publish("patient-group-created", pg);

        _unitOfWork.Complete();

        return pg;
    }

    public void AddPatient(string patientGroupId, Patient patient)
    {
        var pg = Get(patientGroupId);

        _unitOfWork.PatientGroups.AddPatient(pg, patient);
        _unitOfWork.Complete();
    }

    public void AddCaregiver(string patientGroupId, Caregiver caregiver)
    {
        var pg = Get(patientGroupId);

        _unitOfWork.PatientGroups.AddCaregiver(pg, caregiver);
        _unitOfWork.Complete();
    }

    public IEnumerable<PatientGroup> GetAll()
    {
        return _unitOfWork.PatientGroups.GetAll();
    }

    public IEnumerable<Patient> GetPatients(string id)
    {
        var patientGroup = Get(id);
        return patientGroup.PatientGroupPatients.Select(pg => pg.Patient);
    }

    public IEnumerable<Caregiver> GetCaregivers(string id)
    {
        var patientGroup = Get(id);
        return patientGroup.PatientGroupCaregivers.Select(pg => pg.Caregiver);
    }
}