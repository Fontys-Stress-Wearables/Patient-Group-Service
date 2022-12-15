using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Patient_Group_Service.Dtos;
using Patient_Group_Service.Interfaces;

namespace Patient_Group_Service.Controllers;

[ApiController]
[Authorize("p-organization-admin")]
[Route("patient")]
public class PatientController : ControllerBase
{
    private readonly IPatientGroupService _patientGroupService;
    private readonly IMapper _mapper;

    public PatientController(IPatientGroupService patientGroupService, IMapper mapper)
    {
        _patientGroupService = patientGroupService;
        _mapper = mapper;
    }

    [HttpGet("{id}/patient-groups")]
    public IEnumerable<PatientGroupDTO> GetPatientsGroups(string id)
    {
        var groups = _patientGroupService.GetForPatient(id, HttpContext.User.GetTenantId()!);
        return _mapper.Map<IEnumerable<PatientGroupDTO>>(groups);
    }
}