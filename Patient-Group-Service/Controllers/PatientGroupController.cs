using Microsoft.AspNetCore.Mvc;
using Patient_Group_Service.Dtos;
using Patient_Group_Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace Patient_Group_Service.Controllers;

[ApiController]
[Route("patient-groups")]
public class PatientGroupController : ControllerBase
{
    private readonly ILogger<PatientGroupController> _logger;
    private readonly IPatientGroupService _patientGroupService;
    private readonly IPatientService _patientService;
    private readonly ICaregiverService _caregiverService;
    private readonly IMapper _mapper;

    public PatientGroupController
    (
        ILogger<PatientGroupController> logger, IPatientGroupService patientGroupService, 
        IPatientService patientService, ICaregiverService caregiverService, IMapper mapper
    )
    {
        _logger = logger;
        _patientGroupService = patientGroupService;
        _patientService = patientService;
        _caregiverService = caregiverService;
        _mapper = mapper;
    }

    [HttpGet]
    public IEnumerable<PatientGroupDTO> GetPatientGroups()
    {
        var groups = _patientGroupService.GetAll();

        return _mapper.Map<IEnumerable<PatientGroupDTO>>(groups);
    }
    [HttpGet("{id}")]
    public PatientGroupDTO GetPatientGroupById(string id)
    {
        
        var group = _patientGroupService.Get(id);

        return _mapper.Map <PatientGroupDTO>(group);
    }
    [Authorize("p-organization-admin")]
    [HttpPost("{id}/patients")]
    public void PostPatientToPatientGroup(string id, [FromBody] string patientId)
    {
        _patientGroupService.AddPatient(id, patientId);
    }
    [Authorize("p-organization-admin")]
    [HttpPost("{id}/caregivers")]
    public void PostCaregiverToPatientGroup(string id, [FromBody] string caregiverId)
    {
        _patientGroupService.AddCaregiver(id, caregiverId);
    }

    [HttpGet("{id}/caregivers")]
    public IEnumerable<CaregiverDTO> GetCaregiversByPatientGroup(string id)
    {
        var caregivers = _patientGroupService.GetCaregivers(id);

        return _mapper.Map<IEnumerable<CaregiverDTO>>(caregivers);
    }

    [HttpGet("{id}/patients")]
    public IEnumerable<PatientDTO> GetPatients(string id)
    {
        var patients = _patientGroupService.GetPatients(id);

        return _mapper.Map<IEnumerable<PatientDTO>>(patients);
    }
    
    [Authorize("p-organization-admin")]
    [HttpPost]
    public PatientGroupDTO PostPatientGroup(CreatePatientGroupDTO patientGroup)
    {
        var newGroup = _patientGroupService.Create(patientGroup.GroupName, patientGroup.Description);

        return _mapper.Map<PatientGroupDTO>(newGroup);
    }
    
    [Authorize("p-organization-admin")]
    [HttpDelete("{id}")]
    public void DeletePatientGroup(string id)
    {
        _patientGroupService.DeletePatientgroup(id);
    }
}
