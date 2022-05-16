using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Patient_Group_Service.Dtos;
using Patient_Group_Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace Patient_Group_Service.Controllers;

[ApiController]
[Route("patient-groups")]
public class PatientGroupController : ControllerBase
{
    private readonly IPatientGroupService _patientGroupService;
    private readonly IMapper _mapper;
    
    public PatientGroupController(IPatientGroupService patientGroupService, IMapper mapper)
    {
        _patientGroupService = patientGroupService;
        _mapper = mapper;
    }

    [HttpGet]
    public IEnumerable<PatientGroupDTO> GetPatientGroups()
    {
        var groups = _patientGroupService.GetAll(HttpContext.User.GetTenantId()!);

        return _mapper.Map<IEnumerable<PatientGroupDTO>>(groups);
    }
    
    [HttpGet("{id}")]
    public PatientGroupDTO GetPatientGroupById(string id)
    {
        
        var group = _patientGroupService.Get(id,HttpContext.User.GetTenantId()!);

        return _mapper.Map <PatientGroupDTO>(group);
    }
    [Authorize("p-organization-admin")]
    [HttpPost("{id}/patients")]
    public void PostPatientToPatientGroup(string id, [FromBody] string patientId)
    {
        _patientGroupService.AddPatient(id, patientId,HttpContext.User.GetTenantId()!);
    }
    [Authorize("p-organization-admin")]
    [HttpPost("{id}/caregivers")]
    public void PostCaregiverToPatientGroup(string id, [FromBody] string caregiverId)
    {
        _patientGroupService.AddCaregiver(id, caregiverId, HttpContext.User.GetTenantId()!);
    }

    [HttpGet("{id}/caregivers")]
    public IEnumerable<CaregiverDTO> GetCaregiversByPatientGroup(string id)
    {
        var caregivers = _patientGroupService.GetCaregivers(id,HttpContext.User.GetTenantId()!);

        return _mapper.Map<IEnumerable<CaregiverDTO>>(caregivers);
    }

    [HttpGet("{id}/patients")]
    public IEnumerable<PatientDTO> GetPatients(string id)
    {
        var patients = _patientGroupService.GetPatients(id,HttpContext.User.GetTenantId()!);

        return _mapper.Map<IEnumerable<PatientDTO>>(patients);
    }
    
    [Authorize("p-organization-admin")]
    [HttpPost]
    public PatientGroupDTO PostPatientGroup(CreatePatientGroupDTO patientGroup)
    {
        var newGroup = _patientGroupService.Create(patientGroup.GroupName, patientGroup.Description,HttpContext.User.GetTenantId()!);

        return _mapper.Map<PatientGroupDTO>(newGroup);
    }
    
    [Authorize("p-organization-admin")]
    [HttpPut("{id}")]
    public PatientGroupDTO UpdatePatientGroup(string id, [FromBody] UpdatePatientGroupDTO patientGroup)
    {
        var updatedGroup = _patientGroupService.Update(id, patientGroup.GroupName, patientGroup.Description,HttpContext.User.GetTenantId()!);

        return _mapper.Map<PatientGroupDTO>(updatedGroup);
    }

    [Authorize("p-organization-admin")]
    [HttpDelete("{id}/patient")]
    public void RemovePatientFromPatientGroup(string id, [FromBody] string patientId)
    {
        _patientGroupService.RemovePatient(id, patientId,HttpContext.User.GetTenantId()!);
    }
    
    [Authorize("p-organization-admin")]
    [HttpDelete("{id}/caregiver")]
    public void RemoveCaregiverFromPatientGroup(string id, [FromBody] string caregiverId)
    {
        _patientGroupService.RemoveCaregiver(id, caregiverId,HttpContext.User.GetTenantId()!);
    }
    
    [Authorize("p-organization-admin")]
    [HttpDelete("{id}")]
    public void DeletePatientGroup(string id)
    {
        _patientGroupService.Delete(id,HttpContext.User.GetTenantId()!);
    }
}
