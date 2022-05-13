using Microsoft.Graph;
using Microsoft.Identity.Web;
using Patient_Group_Service.Exceptions;
using Patient_Group_Service.Interfaces;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Services;

public class PatientGroupService : IPatientGroupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INatsService _natsService;
    private readonly IConfiguration _configuration;
        
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly GraphServiceClient _graphServiceClient;

    public PatientGroupService(IUnitOfWork unitOfWork, INatsService natsService, ITokenAcquisition tokenAcquisition, GraphServiceClient graphServiceClient, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _natsService = natsService;
        _tokenAcquisition = tokenAcquisition;
        _graphServiceClient = graphServiceClient;
        _configuration = configuration;
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

    public void AddPatient(string patientGroupId, string patientId)
    {
        var patientGroup = Get(patientGroupId);
        
        var patient = _unitOfWork.Patients.GetById(patientId);

        if (patient == null)
        {
            throw new BadRequestException($"Patient with id '{patientId}' doesn't exist.");
        }

        _unitOfWork.PatientGroups.AddPatient(patientGroup, patient);

        _natsService.Publish("patient-group-patient-added", new
        {
            patientId = patientId,
            patientGroupId = patientGroupId
        });

        _unitOfWork.Complete();
    }

    public async Task AddCaregiver(string patientGroupId, string caregiverId)
    {
        var patientGroup = Get(patientGroupId);

        var caregiver = _unitOfWork.Caregivers.GetById(caregiverId);

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

        _natsService.Publish("patient-group-caregiver-added", new
        {
            caregiverId, patientGroupId
        });

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

    public void DeletePatientgroup(string id)
    {
        var group = _unitOfWork.PatientGroups.GetById(id);

        if (group == null)
        {
            throw new NotFoundException($"Patient group with id '{id}' doesn't exist.");
        }

        _unitOfWork.PatientGroups.Remove(group);
        
        _natsService.Publish("patient-group-removed", new
        {
            patientGroupId = id
        });
        
        _unitOfWork.Complete();
    }
}